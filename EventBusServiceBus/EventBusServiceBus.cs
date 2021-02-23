using Autofac;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost;
using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusServiceBus
{
    public class EventBusServiceBus : IAsyncEventBus,IAsyncDisposable
    {
        private readonly IServiceBusPersisterConnection _serviceBusPersisterConnection;
        private readonly ILogger<EventBusServiceBus> _logger;
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly ServiceBusProcessor _serviceBusProcessor;
        private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "innermost_event_bus";
        /// <summary>
        /// 对于所有继承于IntegrationEvent的事件，都以该后缀结尾
        /// </summary>
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";
        private const string TOPIC_NAME = "innermost_event_bus";
        private bool _disposed;

        public EventBusServiceBus(IServiceBusPersisterConnection serviceBusPersisterConnection,ILogger<EventBusServiceBus> logger,
            IEventBusSubscriptionManager subscriptionManager,string subscriptionName, ILifetimeScope autofac)
        {
            _serviceBusPersisterConnection = serviceBusPersisterConnection;
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _subscriptionManager = subscriptionManager ?? new InMemoryEventBusSubscriptionsManager();
            _serviceBusProcessor = _serviceBusPersisterConnection.CreateModel().CreateProcessor(TOPIC_NAME,subscriptionName,
                new ServiceBusProcessorOptions 
                { 
                    MaxConcurrentCalls=10,
                    AutoCompleteMessages=false
                }
            );
            _serviceBusAdministrationClient = _serviceBusPersisterConnection.CreateAdministrationModel();
            _autofac = autofac;

            RemoveDefaultFilter().GetAwaiter().GetResult();
            RegisterProcessorMessageHandler();
        }

        public async Task Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
            var eventJsonStr = JsonConvert.SerializeObject(@event);
            var messageBody = new BinaryData(eventJsonStr);

            ServiceBusMessage message = new ServiceBusMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = messageBody,
                Subject = eventName
            };

            await using (var sender = _serviceBusPersisterConnection.CreateModel().CreateSender(TOPIC_NAME))
            {
                await sender.SendMessageAsync(message);
            }
        }

        public async Task Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
            var subscriptionName = typeof(TH).Name;

            var containsEvent = _subscriptionManager.HasSubscriptionForEvent<T>();
            if(!containsEvent)
            {
                try
                {
                    await _serviceBusAdministrationClient.CreateRuleAsync(TOPIC_NAME, subscriptionName, new CreateRuleOptions
                    {
                        Filter=new CorrelationRuleFilter { Subject=eventName},
                        Name=eventName
                    });
                }
                catch (ServiceBusException)
                {
                    _logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
                }
            }

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            _subscriptionManager.AddSubscription<T, TH>();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            _subscriptionManager.AddDynamicSubscription<TH>(eventName);

        }

        public async Task UnSubsribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
            var subscriptionName = typeof(TH).Name;

            try
            {
                await _serviceBusAdministrationClient.DeleteRuleAsync(TOPIC_NAME, subscriptionName, eventName);
            }
            catch (ServiceBusException)
            {
                _logger.LogWarning("The messaging entity {eventName} Could not be found.", eventName);
            }

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subscriptionManager.RemoveSubScription<T, TH>();
        }

        public void UnSubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subscriptionManager.RemoveDynamicSubScription<TH>(eventName);
        }

        private async Task RemoveDefaultFilter()
        {
            var subscriptions = _serviceBusAdministrationClient.GetSubscriptionsAsync(TOPIC_NAME).GetAsyncEnumerator();
            try
            {
                while (await subscriptions.MoveNextAsync())
                {
                    await _serviceBusAdministrationClient.DeleteRuleAsync(TOPIC_NAME, subscriptions.Current.SubscriptionName, CreateRuleOptions.DefaultRuleName);
                }
            }
            catch (ServiceBusException)
            {
                _logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", CreateRuleOptions.DefaultRuleName);
            }
        }
        /// <summary>
        /// 往processor的委托里传入处理消息和处理错误的函数，在发送一个message后，processor会invoke委托。
        /// </summary>
        private void RegisterProcessorMessageHandler()
        {
            _serviceBusProcessor.ProcessMessageAsync += ProcessMessage;
            _serviceBusProcessor.ProcessErrorAsync += ProcessError;
        }
        
        private async Task ProcessMessage(ProcessMessageEventArgs messageArgs)
        {
            //在subscriptionsManager中，eventName通过Type决定，没有去掉后缀
            var eventClassName = messageArgs.Message.Subject + INTEGRATION_EVENT_SUFFIX;
            var messageData = messageArgs.Message.Body.ToString();

            if(await ProcessEvent(eventClassName,messageData))
            {
                await messageArgs.CompleteMessageAsync(messageArgs.Message);
            }
        }

        private async Task<bool> ProcessEvent(string eventName,string messageData)
        {
            bool processed = false;

            if(_subscriptionManager.HasSubscriptionForEvent(eventName))
            {
                using(var scope=_autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if(subscription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            if (handler == null) continue;//没有往autofac注册该种handler
                            dynamic eventData = JObject.Parse(messageData);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            if (handler == null) continue;
                            var eventType = _subscriptionManager.GetEventTypeByName(eventName);//因为模板实现的handler类需要对应事件的类型
                            var integrationEvent = JsonConvert.DeserializeObject(messageData, eventType);
                            var handlerConcreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);//将对应事件的类型填入模板组成完整的handler类型
                            await (Task) handlerConcreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
                processed = true;
            }
            return processed;
        }

        private Task ProcessError(ProcessErrorEventArgs errorArgs)
        {
            var ex = errorArgs.Exception;
            var context = errorArgs.ErrorSource;

            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - ErrorSource: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            _subscriptionManager.Clear();
            try
            {
                await _serviceBusPersisterConnection.DisposeAsync();
                if(!_serviceBusProcessor.IsClosed)
                {
                    await _serviceBusProcessor.CloseAsync();
                }
                await _serviceBusProcessor.DisposeAsync();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
