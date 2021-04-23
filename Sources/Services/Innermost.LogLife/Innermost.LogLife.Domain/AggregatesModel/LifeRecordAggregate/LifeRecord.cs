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
        : Entity, IAggregateRoot
    {
        private string _userId;
        public string UserId => _userId;
        public string Title { get; private set; }
        public string Text { get; private set; }
        public TextType TextType { get; private set; }
        public int TextTypeId => _textTypeId;
        private int _textTypeId;
        public Location Location { get; private set; }
        public DateTime PublishTime { get; private set; }
        public MusicRecord MusicRecord { get; private set; }
        public int MusicRecordId => _musicRecordId;
        private int _musicRecordId;
        //public IEnumerable<Image> Images { get; set; }//TODO
        public IEnumerable<EmotionTag> EmotionTags { get; private set; }
        /// <summary>
        /// Document Path.User can customer the loglife document structrue.
        /// </summary>
        public string Path { get; private set; }

        private bool _isShared;

        private IEnumerable<dynamic> PropertiesCannotUpdateByUpdateOneRecordCommand()
        {
            yield return Id;
            yield return Path;
            yield return PublishTime;
        }
            
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
            _isShared = false;
            if(isShared)
            {
                SetRecordShared();
            }
        }

        public void SetRecordShared()
        {
            if(!_isShared)
            {
                AddDomainEvent(new ToMakeRecordSharedDomainEvent(this.Id));
                _isShared = true;
            }
        }

        public void SetRecordPrivate()
        {
            if(_isShared)
            {
                AddDomainEvent(new ToMakeRecordPrivateDomainEvent(this.Id));
                _isShared = false;
            }
        }

        public void SetEmotionTags(IEnumerable<EmotionTag> emotionTags)
        {
            if(!emotionTags.EqualList(this.EmotionTags))
            {
                AddDomainEvent(new ToChangeEmotionTagsDomainEvent(this.Id));
                EmotionTags = emotionTags;
            }
        }

        public bool IsValidatedToUpdate(LifeRecord recordToUpdate)
        {
            var changeValidatedTag = this.PropertiesCannotUpdateByUpdateOneRecordCommand().SequenceEqual(recordToUpdate.PropertiesCannotUpdateByUpdateOneRecordCommand());
            if (!changeValidatedTag)
            {
                return false;
            }
            return true;
        }

        IEnumerable<EmotionTag> GetEmotionTagsByPrediction()
        {
            //TODO 发布领域事件，让Handler去请求集成事件
            return new List<EmotionTag>();
        }


    }
}
