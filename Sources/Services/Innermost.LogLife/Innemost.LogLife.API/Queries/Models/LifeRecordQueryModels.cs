using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Queries.Model
{
    public record LifeRecord
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Text { get; init; }
        public TextType TextType { get; set; }
        public Location Location { get; set; }
        public DateTime PublishTime { get; init; }
        public MusicRecord MusicRecord { get; set; }
        public IEnumerable<EmotionTag> EmotionTags { get; init; }
        public string Path { get; init; }
        public bool IsShared { get; init; }
    }

    public record TextType
    {
        public string TextTypeName { get; init; }
    }

    public record Location
    {
        public string Province { get; init; }
        public string City { get; init; }
        public string County { get; init; }
        public string Town { get; init; }
        public string Place { get; init; }
    }

    public record MusicRecord
    {
        public string MusicName { get; init; }
        public string Singer { get; init; }
        public string Album { get; init; }
    }

    public record EmotionTag
    {

    }
}
