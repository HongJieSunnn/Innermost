using Innemost.LogLife.API.Application.IntegrationEvents;
using Innemost.LogLife.API.Application.IntegrationEvents.ToMakeRecordPrivate;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.LogLife.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.DomainEventHandlers.ToMakeRecordPrivate
{
    public class ToMakeRecordPrivateDomainEventHandler
        : INotificationHandler<ToMakeRecordPrivateDomainEvent>
    {
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly ILogLifeIntegrationEventService _logLifeIntegrationEventService;
        private readonly ILogger<ToMakeRecordPrivateDomainEvent> _logger;
        public ToMakeRecordPrivateDomainEventHandler(ILifeRecordRepository lifeRecordRepository, ILogLifeIntegrationEventService logLifeIntegrationEventService, ILogger<ToMakeRecordPrivateDomainEvent> logger)
        {
            _lifeRecordRepository = lifeRecordRepository?? throw new ArgumentNullException(nameof(lifeRecordRepository));
            _logLifeIntegrationEventService = logLifeIntegrationEventService?? throw new ArgumentNullException(nameof(logLifeIntegrationEventService));
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(ToMakeRecordPrivateDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Record with recoidId:{notification.RecordId} has been private");

            var record =await _lifeRecordRepository.GetRecordByIdAsync(notification.RecordId);

            var toMakeRecordPrivateIntegrationEvent = new ToMakeRecordPrivateIntegrationEvent(notification.RecordId);

            await _logLifeIntegrationEventService.AddAndSaveEventAsync(toMakeRecordPrivateIntegrationEvent);
        }
    }
}
