using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusInnermost.Events
{
    /// <summary>
    /// 事件的基类。它的子类都是事件总线可处理的事件。
    /// </summary>
    public record IntegrationEvent
    {
        [JsonProperty]
        public Guid Id { get; private init; }

        [JsonProperty]
        public DateTime CreationDate { get; private init; }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id,DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}
