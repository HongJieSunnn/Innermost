using Innermost.LogLife.Domain.Events;
using Innermost.SeedWork;
using Innermost.LogLife.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class LifeRecord
        :Entity,IAggregateRoot
    {
        private string _userId;
        public string UserId { get; }
        public string Title { get; private set; }
        public string Text { get; private set; }
        public TextType TextType { get;private set; }
        private int _textTypeId;
        public Location Location { get; private set; }
        public DateTime PublishTime { get; private set; }
        public MusicRecord MusicRecord { get;private set; }
        private int _musicRecordId;
        //public IEnumerable<Image> Images { get; set; }//TODO
        public IEnumerable<EmotionTag> EmotionTags { get;private set; }
        /// <summary>
        /// Document Path.User can customer the loglife document structrue.
        /// </summary>
        public string Path { get;private set; }

        protected LifeRecord()
        {
            EmotionTags = new List<EmotionTag>();
        }

        public LifeRecord(string userId,string title,string text, TextType textType, bool isShared=false,string path="/",Location location=null,MusicRecord musicRecord=null,IEnumerable<EmotionTag> emotionTags=null)
        {
            _userId = userId;
            Title = title;
            Text = text;
            Path = path;
            TextType=textType;
            _textTypeId = TextType.Id;
            Location = location;
            MusicRecord = musicRecord;//TODO 也许可以某些步骤来获取当前用户正在听的歌
            _musicRecordId = MusicRecord.Id;
            EmotionTags = emotionTags;
        }

        public void SetRecordShared()
        {
            AddDomainEvent(new ToMakeRecordSharedDomainEvent(this.Id));
        }

        public void SetRecordPrivate()
        {
            //该方法应该放在分享的Record实体下。
        }

        public void SetEmotionTags(IEnumerable<EmotionTag> emotionTags)
        {
            if(!emotionTags.EqualList(this.EmotionTags))
            {
                AddDomainEvent(new ToChangeEmotionTagsDomainEvent(this.Id));
                EmotionTags = emotionTags;
            }
        }



        IEnumerable<EmotionTag> GetEmotionTagsByPrediction()
        {
            //TODO 发布领域事件，让Handler去请求集成事件
            return new List<EmotionTag>();
        }


    }
}
