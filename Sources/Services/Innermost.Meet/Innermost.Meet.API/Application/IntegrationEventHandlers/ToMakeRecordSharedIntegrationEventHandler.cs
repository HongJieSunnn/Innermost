using Innemost.LogLife.API.Application.IntegrationEvents.ToMakeRecordShared;
using Innermost.EventBusInnermost.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Meet.API.Application.IntegrationEventHandlers
{
    public class ToMakeRecordSharedIntegrationEventHandler
        : IIntegrationEventHandler<ToMakeRecordSharedIntegrationEvent>
    {
        public Task Handle(ToMakeRecordSharedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
