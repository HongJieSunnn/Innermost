using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure.Repositories
{
    public class LifeRecordRepository
        : ILifeRecordRepository
    {
        private readonly LifeRecordDbContext _db;
        public IUnitOfWork UnitOfWork => _db;

        public LifeRecord Add(LifeRecord lifeRecord)
        {
            return _db.Add<LifeRecord>(lifeRecord).Entity;
        }

        public Task<LifeRecord> AddAsync(LifeRecord lifeRecord)
        {
            throw new NotImplementedException();
        }

        public LifeRecord Delete(LifeRecord lifeRecord)
        {
            throw new NotImplementedException();
        }

        public Task<LifeRecord> FindRecordByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LifeRecord>> FindRecordsByPathAsync(string userId, string path)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LifeRecord>> FindRecordsByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public LifeRecord Update(LifeRecord lifeRecord)
        {
            throw new NotImplementedException();
        }
    }
}
