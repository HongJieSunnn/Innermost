using Innemost.LogLife.API.Services.FactoryServices;
using Innermost.LogLife.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Infrastructure.Factories
{
    public class LifeRecordDbContextFactory : IDesignTimeDbContextFactory<LifeRecordDbContext>
    {
        public LifeRecordDbContext CreateDbContext(string[] args)
        {
            var options = FactoryService.GetDbContextOptions<LifeRecordDbContext>();

            return new LifeRecordDbContext(options);
        }
    }
}
