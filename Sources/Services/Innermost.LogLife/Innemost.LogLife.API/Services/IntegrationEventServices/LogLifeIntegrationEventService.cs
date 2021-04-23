using EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Events;
using Innermost.LogLife.Infrastructure;
using IntegrationEventRecord.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.IntegrationEvents
{
    public class LogLifeIntegrationEventService
        : ILogLifeIntegrationEventService
    {
        private readonly IAsyncEventBus _eventBus;
        private readonly ILogger<LogLifeIntegrationEventService> _logger;
        private readonly LifeRecordDbContext _lifeRecordDbContext;
        private readonly IntegrationEventRecordServiceFactory _integrationEventRecordServiceFactory;
        private readonly IIntegrationEventRecordService _integrationEventRecordService;
        public LogLifeIntegrationEventService(
            IAsyncEventBus eventBus,ILogger<LogLifeIntegrationEventService> logger,LifeRecordDbContext lifeRecordDbContext,
            IntegrationEventRecordServiceFactory integrationEventRecordServiceFactory
            )
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
            _lifeRecordDbContext = lifeRecordDbContext ?? throw new ArgumentNullException(nameof(LifeRecordDbContext));
            _integrationEventRecordServiceFactory = integrationEventRecordServiceFactory ?? throw new ArgumentNullException(nameof(IntegrationEventRecordServiceFactory));
            _integrationEventRecordService = _integrationEventRecordServiceFactory.NewService(_lifeRecordDbContext.Database.GetDbConnection());
        }
        
        public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", integrationEvent.Id, integrationEvent);
            await _integrationEventRecordService.SaveEventAsync(integrationEvent, _lifeRecordDbContext.CurrentTransaction);
        }

        public async Task PublishEventsAsync(Guid transactionId)
        {
            var recordsToPublish = await _integrationEventRecordService.RetrieveEventsByEventContentsToPublishAsync(transactionId);
            foreach(var record in recordsToPublish)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", record.EventId, Program.AppName, record.IntegrationEvent);

                try
                {
                    await _integrationEventRecordService.MarkEventAsInProcessAsync(transactionId);
                    await _eventBus.Publish(record.IntegrationEvent);
                    await _integrationEventRecordService.MarkEventAsPublishedAsync(transactionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", record.EventId, Program.AppName);
                    throw;
                }
            }
        }
    }
}
