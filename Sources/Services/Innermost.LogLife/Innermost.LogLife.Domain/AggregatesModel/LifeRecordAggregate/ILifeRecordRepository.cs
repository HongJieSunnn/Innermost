using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public interface ILifeRecordRepository
        :IRepository<LifeRecord>
    {
        LifeRecord Add(LifeRecord lifeRecord);
        Task<LifeRecord> AddAsync(LifeRecord lifeRecord);
        LifeRecord Update(LifeRecord lifeRecord);
        LifeRecord Delete(LifeRecord lifeRecord);
        Task<IEnumerable<LifeRecord>> FindRecordsByUserIdAsync(string userId);
        Task<IEnumerable<LifeRecord>> FindRecordsByPathAsync(string userId,string path);
        Task<LifeRecord> FindRecordByIdAsync(string id);
    }
}
