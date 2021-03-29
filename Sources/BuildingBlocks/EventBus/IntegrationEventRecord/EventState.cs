using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord
{
    public class EventState
    {
        public string StateName { get;private init; }
        public static EventState NotPublished = new EventState("NotPublished");
        public static EventState InProcess = new EventState("InProcess");
        public static EventState Published = new EventState("Published");
        public static EventState PublishedFailed = new EventState("PublishedFailed");

        private EventState(string stateName)
        {
            StateName = stateName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var state = obj as EventState;

            return this.StateName == state.StateName;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return StateName.GetHashCode();
        }
    }
}
