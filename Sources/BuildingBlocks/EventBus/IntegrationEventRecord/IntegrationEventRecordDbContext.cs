using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord
{
    public class IntegrationEventRecordDbContext:DbContext
    {
        public DbSet<IntegrationEventRecord> IntegrationEventRecords { get; set; }
        public IntegrationEventRecordDbContext(DbContextOptions<IntegrationEventRecordDbContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEventRecordEntityTypeConfiguration());
        }
    }
}
