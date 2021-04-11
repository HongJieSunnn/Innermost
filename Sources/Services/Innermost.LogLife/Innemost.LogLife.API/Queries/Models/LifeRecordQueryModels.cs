using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Queries.Model
{
    public record RecordSummary
    {
        public string Title { get; init; }
        public string Text { get; init; }
        public DateTime PublishTime { get; init; }
        public IEnumerable<string> EmotionTags { get; init; }
    }

    public record LifeRecord
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Text { get; init; }
        public TextType TextType { get; init; }
        public Location Location { get; init; }
        public DateTime PublishTime { get; init; }
        public MusicRecord MusicRecord { get; init; }
        public IEnumerable<EmotionTag> EmotionTags { get; init; }
        public string Path { get; init; }
        public bool IsShared { get; init; }
    }
}
