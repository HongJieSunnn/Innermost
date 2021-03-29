using Innermost.SeedWork;
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
        public static IEnumerable<EmotionTag> EmotionTags => GetEmotionTags(); //TODO 若心情标签有多种且可以不断增加，那么可能需要用到Redis存储并每次读取
        public string EmotionEmoji { get;private set; }
        public static EmotionTag Normal = new EmotionTag(1, "NORMAL", "😃");
        public static EmotionTag Happy = new EmotionTag(2, "HAPPY", "🤣");
        public static EmotionTag Speechless = new EmotionTag(3, "SPEECHLESS", "😅");
        public static EmotionTag Angry = new EmotionTag(4, "ANGRY", "😡");
        public static EmotionTag Depressed = new EmotionTag(5, "DEPRESSED", "😔");
        public static EmotionTag Sad = new EmotionTag(6, "SAD", "😭");

        public LifeRecord LifeRecord { get;private set; }
        private int _lifeRecordId;
        public int LifeRecordId => _lifeRecordId;

        public EmotionTag(int id, string name,string emotionEmoji) : base(id, name)
        {
            EmotionEmoji = emotionEmoji;
        }

        public static IEnumerable<EmotionTag> GetEmotionTags()
        {
            yield return Normal;
            yield return Happy;
            yield return Speechless;
            yield return Angry;
            yield return Depressed;
            yield return Sad;
        }

        public static EmotionTag GetFromName(string name)
        {
            var emotionTag = EmotionTags.SingleOrDefault(n => n.Name.Equals(name));
            if (emotionTag == null)
                throw new ArgumentException($"can not find emotiontag by name:{name}");
            return emotionTag;
        }

        public static EmotionTag GetFromId(int id)
        {
            var emotionTag = EmotionTags.SingleOrDefault(n => n.Id.Equals(id));
            if (emotionTag == null)
                throw new ArgumentException($"can not find emotiontag by id:{id}");
            return emotionTag;
        }

        public static IEnumerable<int> GetIdsFromNames(IEnumerable<string> names)
        {
            HashSet<int> idSet = new HashSet<int>();
            foreach(var name in names)
            {
                var id = GetFromName(name).Id;
                if (!idSet.Contains(id))
                    idSet.Add(id);
            }
            return idSet.ToList();
        }
    }
}
