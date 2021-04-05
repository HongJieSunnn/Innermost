using Innemost.LogLife.API.Application.IntegrationEvents;
using Innemost.LogLife.API.Application.IntegrationEvents.ToMakeRecordShared;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.LogLife.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.DomainEventHandlers.ToMakeRecordShared
{
    public class ToMakeRecordSharedDomainEventHandler
        : INotificationHandler<ToMakeRecordSharedDomainEvent>
    {
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly ILogLifeIntegrationEventService _logLifeIntegrationEventService;
        private readonly ILogger<ToMakeRecordSharedDomainEvent> _logger;
        public ToMakeRecordSharedDomainEventHandler(ILifeRecordRepository lifeRecordRepository,ILogLifeIntegrationEventService logLifeIntegrationEventService,ILogger<ToMakeRecordSharedDomainEvent> logger)
        {
            _lifeRecordRepository = lifeRecordRepository ?? throw new ArgumentNullException(nameof(lifeRecordRepository));
            _logLifeIntegrationEventService = logLifeIntegrationEventService ?? throw new ArgumentNullException(nameof(logLifeIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(ToMakeRecordSharedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Record with recoidId:{notification.RecordId} has been shared");
            LifeRecord record =await _lifeRecordRepository.GetRecordByIdAsync(notification.RecordId);

            var toMakeRecordSharedIntegrationEvent = new ToMakeRecordSharedIntegrationEvent(
                record.Id,record.UserId, record.Title, record.Text, 
                record.TextType, record.Location, record.PublishTime, 
                record.MusicRecord, record.EmotionTags);

            await _logLifeIntegrationEventService.AddAndSaveEventAsync(toMakeRecordSharedIntegrationEvent);
        }
    }
}
