using EventBusInnermost.Abstractions;
using EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusInnermost
{
    /// <summary>
    /// 自定义的订阅管理者
    /// </summary>
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionManager
    {
        /// <summary>
        /// key:eventName value:subscriptions who subscribe this event
        /// </summary>
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;
        public bool IsEmpty => !_handlers.Any();
        public void Clear() => _handlers.Clear();

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public void AddDynamicSubscription<TH>(string eventName) where TH : IIntegrationEventHandler
        {
            DoAddSubscription(typeof(TH), eventName, isDynamic: true);
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler
        {
            var eventName = GetEventKey<T>();
            var eventType = typeof(TH);

            DoAddSubscription(eventType, eventName, isDynamic: false);

            if (!_eventTypes.Contains(eventType))
            {
                _eventTypes.Add(eventType);
            }
        }
        /// <summary>
        /// Do AddSubscription function
        /// </summary>
        /// <param name="handlerType">subscription type</param>
        /// <param name="eventName">eventName</param>
        /// <param name="isDynamic"></param>
        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(sub => sub.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            if (isDynamic)
            {
                _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            }
            else
            {
                _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            }
        }

        public void RemoveDynamicSubScription<TH>(string eventName) where TH : IIntegrationEventHandler
        {
            var subscription = FindDynamicSubscriptionToRemove<TH>(eventName);

            DoRemoveSubScription(eventName, subscription);
        }

        public void RemoveSubScription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler
        {
            var eventName = GetEventKey<T>();
            var subscription = FindSubscriptionToRemove<T, TH>();

            DoRemoveSubScription(eventName, subscription);
        }
        /// <summary>
        /// Do RemoveSubScription function.
        /// </summary>
        /// <param name="eventName">eventName</param>
        /// <param name="subscription">subscription of one event want to be removed</param>
        private void DoRemoveSubScription(string eventName, SubscriptionInfo subscription)
        {
            if (subscription != null)
            {
                _handlers[eventName].Remove(subscription);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }
            }
        }
        /// <summary>
        /// Find Subscription to remove
        /// </summary>
        /// <typeparam name="T">eventType</typeparam>
        /// <typeparam name="TH">handlerType</typeparam>
        /// <returns></returns>
        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            return DoFindSubscriptionToRemove(eventName, handlerType);
        }
        /// <summary>
        /// Find DynamicSubscription to remove
        /// </summary>
        /// <typeparam name="T">eventType</typeparam>
        /// <typeparam name="TH">handlerType</typeparam>
        /// <returns></returns>
        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
            where TH:IIntegrationEventHandler
        {
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }
        /// <summary>
        /// Do FindSubscription to remove
        /// </summary>
        /// <param name="eventName">eventName from eventType</param>
        /// <param name="handlerType">handlerType from template TH</param>
        /// <returns></returns>
        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName,Type handlerType)
        {
            if(!HasSubscriptionForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
        {
            var eventName = typeof(T).Name;
            return HasSubscriptionForEvent(eventName);
        }

        public bool HasSubscriptionForEvent(string eventName)
        {
            return _handlers.ContainsKey(eventName);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var eventName = GetEventKey<T>();
            return GetHandlersForEvent(eventName);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
        /// <summary>
        /// while eventName's appropriate event is being removed.Raise the delegate OnEventRemoved.
        /// </summary>
        /// <param name="eventName"></param>
        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }
    }
}
