using Innermost.EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.IntegrationEvents
{
    public interface ILogLifeIntegrationEventService
    {
        Task PublishEventsAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent integrationEvent);
    }
}
