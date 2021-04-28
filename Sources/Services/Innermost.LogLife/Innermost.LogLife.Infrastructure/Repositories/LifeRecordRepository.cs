using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.SeedWork;
using Microsoft.EntityFrameworkCore;
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
        public LifeRecordRepository(LifeRecordDbContext context)
        {
            _db = context;
        }
        public LifeRecord Add(LifeRecord lifeRecord)
        {
            return _db.Add<LifeRecord>(lifeRecord ?? throw new ArgumentNullException(nameof(lifeRecord))).Entity;
        }

        public async Task<LifeRecord> AddAsync(LifeRecord lifeRecord)
        {
            var entity = await _db.LifeRecords.AddAsync(lifeRecord ?? throw new ArgumentNullException(nameof(lifeRecord)));
            return entity.Entity;
        }

        public LifeRecord Update(LifeRecord lifeRecord)
        {
            return _db.Update(lifeRecord ?? throw new ArgumentNullException(nameof(lifeRecord))).Entity;
        }

        public LifeRecord Delete(LifeRecord lifeRecord)
        {
            return _db.Remove(lifeRecord??throw new ArgumentNullException(nameof(lifeRecord))).Entity;
        }

        public async Task<LifeRecord> GetRecordByIdAsync(int id)
        {
            var record = await _db.LifeRecords.FirstOrDefaultAsync(l=>l.Id==id);//eShopOnContainer直接Include了地址，但我感觉没必要，因为Include应该是所有的Location
            
            if(record==null)
            {
                record = _db.LifeRecords.Local.FirstOrDefault(l => l.Id == id);
            }

            if(record!=null)
            {
                await _db.Entry(record).Reference(l => l.Location).LoadAsync();
                await _db.Entry(record).Reference(l => l.MusicRecord).LoadAsync();
                await _db.Entry(record).Reference(l => l.TextType).LoadAsync();
                await _db.Entry(record).Collection(l => l.EmotionTags).LoadAsync();
            }

            return record;
        }

        public IEnumerable<LifeRecord> DeleteRecordsUnderPath(string path)
        {
            var recordsUnderPath = _db.LifeRecords.Where(l => l.Path == path).ToList();

            if(recordsUnderPath==null)
            {
                recordsUnderPath=_db.LifeRecords.Local.Where(l => l.Path == path).ToList();
            }
            _db.LifeRecords.RemoveRange(recordsUnderPath);

            return recordsUnderPath;
        }
    }
}
