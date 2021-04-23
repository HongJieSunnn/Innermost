using Innermost.EventBusInnermost.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord
{
    public class IntegrationEventRecord
    {
        public Guid EventId { get;private set; }
        public string TransactionId { get;private set; }
        public string EventTypeName { get;private set; }
        public string State { get; set; }
        public DateTime CreateTime { get; private set; }
        public string EventContent { get;private set; }
        public int TimesSend { get; set; }//发送的次数，当该事件已在处理而又发送则TimesSend++
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split(".").Last();//去掉了名称空间的名
        [NotMapped]
        public IntegrationEvent IntegrationEvent { get;private set; }



        public IntegrationEventRecord(IntegrationEvent @event,Guid transactionId)
        {
            EventId = @event.Id;
            TransactionId = transactionId.ToString();
            EventTypeName = @event.GetType().FullName;
            State = EventState.NotPublished;
            CreateTime = @event.CreationDate;
            EventContent = JsonConvert.SerializeObject(@event);
            TimesSend = 0;
        }

        public IntegrationEventRecord DeserializeIntegrationEventFromEventContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(EventContent,type) as IntegrationEvent;
            return this;
        }
    }
}
