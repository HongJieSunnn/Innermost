using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure
{
    public class LifeRecordDbContext : DbContext, IUnitOfWork
    {
        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
