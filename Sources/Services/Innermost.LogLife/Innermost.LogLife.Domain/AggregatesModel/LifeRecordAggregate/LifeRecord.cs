using Innermost.LogLife.Domain.Events;
using Innermost.SeedWork;
using Innermost.LogLife.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int LocationId => _locationId;
        private int _locationId;
        [Column(TypeName = "DATETIME")]
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

        public LifeRecord(string userId,string title,string text, int textTypeId,int locationId,int musicRecordId, bool isShared=false,string path="",DateTime publishTime=default(DateTime),IEnumerable<EmotionTag> emotionTags=null)
        {
            _userId = userId??throw new ArgumentNullException(nameof(userId));
            Title = title??"";
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Path = path;
            _textTypeId = textTypeId;
            _locationId = locationId;

            //musicRecordId 必定不为0，因为它一定是一首存在的音乐，不允许自定义。每次Innermost.MusicHub添加新音乐，就向Innermost.LogLife 的 MusicRecord Table 添加一条记录。
            _musicRecordId = musicRecordId;//TODO 也许可以某些步骤来获取当前用户正在听的歌

            PublishTime = publishTime == default(DateTime) ? DateTime.Now : publishTime;
            EmotionTags = emotionTags;
            _isShared = isShared;
            if (isShared)
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
