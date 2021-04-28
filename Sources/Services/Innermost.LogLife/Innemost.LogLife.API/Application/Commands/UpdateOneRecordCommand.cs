using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    [DataContract]
    public class UpdateOneRecordCommand:IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public string Text { get; private set; }
        [DataMember]
        [Range(1, 2)]
        public int TextTypeId { get; private set; }
        [DataMember]
        public int LocationId { get; private set; }
        [DataMember]
        public string PublishTime { get; private set; }
        [DataMember]
        public int MusicRecordId { get; private set; }
        public IEnumerable<string> EmotionTags { get; private set; }
        [DataMember]
        public string Path { get; private set; }
        [DataMember]
        public bool IsShared { get; private set; }

        public UpdateOneRecordCommand()
        {
            EmotionTags = new List<string>();
        }

        [JsonConstructor]
        public UpdateOneRecordCommand(int id, string title, string text, int textTypeId, bool isShared, string path, string publishTime,int locationId,int musicRecordId, IEnumerable<string> emotionTags):this()
        {
            Id = id;
            Title = title;
            Text = text;
            Path = path;
            TextTypeId = textTypeId;
            IsShared = isShared;
            PublishTime = publishTime;
            MusicRecordId = musicRecordId;
            LocationId = locationId;
            EmotionTags = emotionTags;
        }
    }
}
