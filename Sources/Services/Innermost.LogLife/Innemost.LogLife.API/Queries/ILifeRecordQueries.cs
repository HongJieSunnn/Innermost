using Innemost.LogLife.API.Models;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Queries
{
    public interface ILifeRecordQueries
    {
        Task<IEnumerable<LifeRecord>> FindRecordsByPathAsync(string userId, string path);
        Task<IEnumerable<IGrouping<string, LifeRecord>>> FindRecordsGroupByPathAsync(string userId);
        Task<IEnumerable<LifeRecord>> FindRecordsByPublishTime(string userId, DateTimeToFind dateTime);
        Task<IEnumerable<LifeRecord>> FindRecordsByEmotionTags(string userId, IEnumerable<int> emotionTagIds);
        Task<IEnumerable<LifeRecord>> FindRecordsByKeywordAsync(string userId, string keyword);
    }
}
