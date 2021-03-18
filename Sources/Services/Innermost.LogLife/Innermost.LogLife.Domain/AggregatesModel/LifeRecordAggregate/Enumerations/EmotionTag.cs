﻿using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class EmotionTag
        : Enumeration
    {
        //public static IEnumerable<EmotionTag> EmotionTags = new List<EmotionTag>(); //TODO 若心情标签有多种且可以不断增加，那么可能需要用到Redis存储并每次读取
        public string EmotionEmoji { get;private set; }
        public static EmotionTag Normal = new EmotionTag(1, "NORMAL", "😃");
        public static EmotionTag Happy = new EmotionTag(2, "HAPPY", "🤣");
        public static EmotionTag Speechless = new EmotionTag(3, "SPEECHLESS", "😅");
        public static EmotionTag Angry = new EmotionTag(4, "ANGRY", "😡");
        public static EmotionTag Depressed = new EmotionTag(5, "DEPRESSED", "😔");
        public static EmotionTag Sad = new EmotionTag(6, "SAD", "😭");

        public EmotionTag(int id, string name,string emotionEmoji) : base(id, name)
        {
            EmotionEmoji = emotionEmoji;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            Enumeration tagObj = obj as Enumeration;
            return tagObj.Id.Equals(this.Id);
        }
    }
}
