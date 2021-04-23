using Innemost.LogLife.API.Application.IntegrationEvents;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.LogLife.Domain.Events;
using IntegrationEventRecord.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.DomainEventHandlers.ToChangeEmotionTags
{
    public class ToChangeEmotionTagsDomainEventHandler
        : INotificationHandler<ToChangeEmotionTagsDomainEvent>
    {
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly ILogLifeIntegrationEventService _logLifeIntegrationEventService;
        private readonly ILogger<ToChangeEmotionTagsDomainEvent> _logger;
        public ToChangeEmotionTagsDomainEventHandler(ILifeRecordRepository lifeRecordRepository,ILogLifeIntegrationEventService logLifeIntegrationEventService,ILogger<ToChangeEmotionTagsDomainEvent> logger)
        {
            _lifeRecordRepository = lifeRecordRepository;
            _logLifeIntegrationEventService = logLifeIntegrationEventService;
            _logger = logger;
        }
        public async Task Handle(ToChangeEmotionTagsDomainEvent notification, CancellationToken cancellationToken)
        {
            var record = await _lifeRecordRepository.GetRecordByIdAsync(notification.RecordId);
            //TODO 保存数据库更改？或者一些更改了标签后的其它操作。
            await _lifeRecordRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
