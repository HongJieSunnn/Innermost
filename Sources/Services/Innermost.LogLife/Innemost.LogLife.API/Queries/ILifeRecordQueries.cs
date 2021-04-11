using Innemost.LogLife.API.Models;
using Innemost.LogLife.API.Queries.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Queries
{
    public interface ILifeRecordQueries
    {
        Task<LifeRecord> FindRecordByRecordId(int id);
        Task<IEnumerable<LifeRecord>> FindRecordsByPathAsync(string userId, string path);
        Task<IEnumerable<IGrouping<string, LifeRecord>>> FindRecordsGroupByPathAsync(string userId);
        Task<IEnumerable<LifeRecord>> FindRecordsByPublishTimeAsync(string userId, DateTimeToFind dateTime);
        Task<IEnumerable<LifeRecord>> FindRecordsByEmotionTagsAsync(string userId, IEnumerable<int> emotionTagIds);
        Task<IEnumerable<LifeRecord>> FindRecordsByKeywordAsync(string userId, string keyword);
        Task<IEnumerable<string>> FindPathsOfUserByUserId(string userId);
        Task<bool> IsPathExistedAsync(string userId, string path);
    }
}
