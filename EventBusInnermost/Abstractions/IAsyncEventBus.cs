using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusInnermost.Abstractions
{
    public interface IAsyncEventBus
    {
        Task Publish(IntegrationEvent @event);

        Task Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        Task UnSubsribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler;

        void UnSubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
    }
}
