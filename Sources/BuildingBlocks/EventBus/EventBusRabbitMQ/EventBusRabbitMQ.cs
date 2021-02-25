using Autofac;
using Innermost.EventBusInnermost;
using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus,IDisposable
    {
        const string BROKER_NAME = "INNERMOST_EVENT_BUS";
        const string AUTOFAC_SCOPE_NAME= "INNERMOST_EVENT_BUS";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly ILifetimeScope _autofac;
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly int _retryCount;

        /// <summary>
        /// In rabbitMQ consumer is subscriber
        /// </summary>
        private IModel _consumerChannel;
        private string _queueName; 
        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac,IEventBusSubscriptionManager subscriptionManager,string queueName=null,int retryCount=3)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _subscriptionManager=subscriptionManager?? throw new ArgumentException(nameof(subscriptionManager));
            _autofac = autofac;
            _queueName = queueName;
            _retryCount = retryCount;
            _consumerChannel = CreateConsumerChannel();
            _subscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
        }

        private void SubscriptionManager_OnEventRemoved(object sender, string eventName)
        {
            if(!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(_queueName, BROKER_NAME, eventName);

                if(_subscriptionManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void UnSubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void UnSubsribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        private IModel CreateConsumerChannel()
        {
            if(!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();
            //exchange -> queue. exchange and queue belongs to channel.queue can be more than one.
            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();//因为恢复后，可能队列中有消息要处理，所以要开启消费(消费就是取消息)
            };
            return channel;
        }
        
        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);//TODO
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if(_subscriptionManager.HasSubscriptionForEvent(eventName))
            {
                using (var scope=_autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if(subscription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            if (handler == null) continue;
                            dynamic eventData = JObject.Parse(message);

                            await Task.Yield();//后续的操作可能占时间，所以这里await一个时间为0的任务，让当前线程返回，而后续的操作交给新开的线程去执行
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            if (handler == null) continue;
                            var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }

        public void Dispose()
        {
            if(_consumerChannel!=null)
            {
                _consumerChannel.Dispose();
            }

            _subscriptionManager.Clear();
        }
    }
}
