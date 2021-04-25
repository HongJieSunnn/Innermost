using Innermost.EventBusInnermost.Events;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.IntegrationEvents.ToMakeRecordShared
{
    public record ToMakeRecordSharedIntegrationEvent
        :IntegrationEvent
    {
        /// <summary>
        /// for delete by recordid while one record change to be private.
        /// </summary>
        public int RecordId { get;private set; }

        public string UserId { get; private set; }

        public string Title { get; private set; }

        public string Text { get; private set; }

        public TextType TextType { get; private set; }

        public Location Location { get; private set; }

        public DateTime PublishTime { get; private set; }

        public MusicRecord MusicRecord { get; private set; }

        //public IEnumerable<Image> Images { get; set; }//TODO
        /// <summary>
        /// 因为需要深拷贝所以用List这里
        /// </summary>
        public List<EmotionTag> EmotionTags { get; private set; }

        public ToMakeRecordSharedIntegrationEvent(int recordId,string userId,string title,string text,TextType textType,Location location,DateTime publishTime,MusicRecord musicRecord,IEnumerable<EmotionTag> emotionTags)
        {
            RecordId = recordId;
            UserId = userId;
            Title = title;
            Text = text;
            TextType = TextType.GetFromId(textType.Id);
            Location = new Location(location);
            PublishTime = publishTime;
            MusicRecord = new MusicRecord(musicRecord);
            EmotionTags = new List<EmotionTag>();
            while (emotionTags.GetEnumerator().MoveNext())
            {
                EmotionTags.Add(emotionTags.GetEnumerator().Current);
            }
        }
    }
}
