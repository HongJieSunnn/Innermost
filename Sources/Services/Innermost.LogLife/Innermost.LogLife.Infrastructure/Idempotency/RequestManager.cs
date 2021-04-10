using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure.Idempotency
{
    public class RequestManager
        : IRequestManager
    {
        private readonly LifeRecordDbContext _db;
        public RequestManager(LifeRecordDbContext context)
        {
            _db = context;
        }
        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exist = await ExistAsync(id);

            var request = exist ? throw new Exception()//TODO DomainException
                : new ClientRequest
                {
                    Id = id,
                    Name = nameof(T),
                    Time = DateTime.UtcNow
                };

            _db.ClientRequests.Add(request);

            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _db.FindAsync<ClientRequest>(id);

            return request != null;
        }
    }
}
