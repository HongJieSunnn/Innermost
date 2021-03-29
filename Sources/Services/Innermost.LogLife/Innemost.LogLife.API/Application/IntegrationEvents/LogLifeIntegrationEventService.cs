using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusInnermost.Events;
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
        private readonly IEventBus _eventBus;
        private readonly ILogger<LogLifeIntegrationEventService> _logger;
        public LogLifeIntegrationEventService(IEventBus eventBus,ILogger<LogLifeIntegrationEventService> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
        }
        //TODO command 通过AddAndSaveEventAsync往数据库加同一个事务的Event，然后把同一个事务的event一起处理掉
        //需要IntegrationEventLog
        public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent)
        {
            //TODO
            throw new NotImplementedException();
        }

        public async Task PublishEventsAsync(Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
