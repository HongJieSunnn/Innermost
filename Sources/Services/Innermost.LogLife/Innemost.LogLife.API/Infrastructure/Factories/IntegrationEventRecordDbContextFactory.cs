using Innemost.LogLife.API.Services.FactoryServices;
using IntegrationEventRecord;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Infrastructure.Factories
{
    public class IntegrationEventRecordDbContextFactory : IDesignTimeDbContextFactory<IntegrationEventRecordDbContext>
    {
        public IntegrationEventRecordDbContext CreateDbContext(string[] args)
        {
            var options = FactoryService.GetDbContextOptions<IntegrationEventRecordDbContext>();

            return new IntegrationEventRecordDbContext(options);
        }
    }
}
