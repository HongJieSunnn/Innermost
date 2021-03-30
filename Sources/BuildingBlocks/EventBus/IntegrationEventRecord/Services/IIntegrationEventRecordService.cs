using Innermost.EventBusInnermost.Events;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord.Services
{
    public interface IIntegrationEventRecordService
    {
        Task<IEnumerable<IntegrationEventRecord>> RetrieveEventsByEventContentsToPublishAsync(Guid transactionId);
        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsInProcessAsync(Guid eventId);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsPublishedFailedAsync(Guid eventId);
    }
}
