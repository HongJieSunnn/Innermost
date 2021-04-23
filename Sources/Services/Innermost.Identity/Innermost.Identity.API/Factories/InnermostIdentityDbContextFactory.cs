using Innermost.Identity.API.Data;
using Innermost.Identity.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Factories
{
    public class InnermostIdentityDbContextFactory : IDesignTimeDbContextFactory<InnermostIdentityDbContext>
    {
        public InnermostIdentityDbContext CreateDbContext(string[] args)
        {
            var options = FactoryService.GetDbContextOptionsMySQL<InnermostIdentityDbContext>();

            return new InnermostIdentityDbContext(options);
        }
    }
}
