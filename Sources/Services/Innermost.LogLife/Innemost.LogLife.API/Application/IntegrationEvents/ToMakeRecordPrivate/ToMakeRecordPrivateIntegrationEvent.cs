using Innermost.EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.IntegrationEvents.ToMakeRecordPrivate
{
    public record ToMakeRecordPrivateIntegrationEvent
        :IntegrationEvent
    {
        public int RecordId { get;private set; }

        public ToMakeRecordPrivateIntegrationEvent(int recordId)
        {
            RecordId = recordId;
        }
    }
}
