using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Innermost.EventBusInnermost.InMemoryEventBusSubscriptionsManager;

namespace Innermost.EventBusInnermost
{
    /// <summary>
    /// 事件和订阅者的管理者接口。它只管理事件和管理者间的关系。
    /// </summary>
    public interface IEventBusSubscriptionManager
    {
        /// <summary>
        /// event and subscription wheher is empty
        /// </summary>
        public bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler;

        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void RemoveSubScription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler;

        void RemoveDynamicSubScription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        /// <summary>
        /// Get event's type name from T
        /// </summary>
        /// <typeparam name="T">event type</typeparam>
        /// <returns></returns>
        string GetEventKey<T>();
    }
}
