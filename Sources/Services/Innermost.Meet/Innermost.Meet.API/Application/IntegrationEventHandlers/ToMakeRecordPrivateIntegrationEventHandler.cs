using Innemost.LogLife.API.Application.IntegrationEvents.ToMakeRecordPrivate;
using Innermost.EventBusInnermost.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Meet.API.Application.IntegrationEventHandlers
{
    public class ToMakeRecordPrivateIntegrationEventHandler
        : IIntegrationEventHandler<ToMakeRecordPrivateIntegrationEvent>
    {
        public Task Handle(ToMakeRecordPrivateIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
