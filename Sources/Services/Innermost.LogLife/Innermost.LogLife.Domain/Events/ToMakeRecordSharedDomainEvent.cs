using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.Events
{
    public class ToMakeRecordSharedDomainEvent
        :INotification
    {
        public int RecordId { get;private set; }
        public ToMakeRecordSharedDomainEvent(int recordId)
        {
            RecordId = recordId;
        }
    }
}
