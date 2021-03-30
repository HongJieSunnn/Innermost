using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord
{
    public class IntegrationEventRecordEntityTypeConfiguration : IEntityTypeConfiguration<IntegrationEventRecord>
    {
        public void Configure(EntityTypeBuilder<IntegrationEventRecord> builder)
        {
            builder.ToTable("IntegrationEventRecord");

            builder.HasKey(i => i.EventId);

            builder
                .Property(i => i.EventContent)
                .IsRequired();

            builder
                .Property(i => i.State)
                .IsRequired();

            builder
                .Property(i => i.CreateTime)
                .IsRequired();

            builder
                .Property(i => i.EventTypeName)
                .IsRequired();

            builder
                .Property(i => i.TimesSend)
                .IsRequired();

            builder
                .Property(i => i.CreateTime)
                .IsRequired();

            //transactionid can be empty
        }
    }
}
