﻿using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    [DataContract]
    public class CreateOneRecordCommand:IRequest<bool>
    {
        /// <summary>
        /// [DataMember] ensure converter can convert private property
        /// </summary>
        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public string Text { get; private set; }
        [DataMember]
        [Range(1,2)]
        public int TextType { get; private set; }
        [DataMember]
        public string Province { get;private set; }
        [DataMember]
        public string City { get; private set; }
        [DataMember]
        public string County { get; private set; }
        [DataMember]
        public string Town { get; private set; }
        [DataMember]
        public string Place { get; private set; }
        [DataMember]
        public string PublishTime { get; private set; }
        [DataMember]
        public int MusicId { get; private set; }
        [DataMember]
        public string MusicName { get; private set; }
        [DataMember]
        public string Singer { get; private set; }
        [DataMember]
        public string Album { get; private set; }
        public IEnumerable<string> EmotionTags { get; private set; }
        [DataMember]
        public string Path { get; private set; }
        [DataMember]
        public bool IsShared { get; private set; }

        public CreateOneRecordCommand()
        {
            EmotionTags = new List<string>();
        }

        public CreateOneRecordCommand(string title, string text, int textType, bool isShared , string path ,
            string province,string city,string county,string town,string place,string publishTime,
            int musicId,string musicName,string singer,string album,IEnumerable<string> emotionTags):this()
        {
            Title = title;
            Text = text;
            Path = path;
            TextType = textType;
            IsShared = isShared;
            Province = province;
            City = city;
            County = county;
            Town = town;
            Place = place;
            PublishTime = publishTime;
            MusicId = musicId;
            MusicName = musicName;
            Singer = singer;
            Album = album;
            EmotionTags = emotionTags;
        }
    }
}
