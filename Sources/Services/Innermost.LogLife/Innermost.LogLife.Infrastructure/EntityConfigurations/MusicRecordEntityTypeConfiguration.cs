using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure.EntityConfigurations
{
    public class MusicRecordEntityTypeConfiguration
        : IEntityTypeConfiguration<MusicRecord>
    {
        public void Configure(EntityTypeBuilder<MusicRecord> builder)
        {
            builder.ToTable("MusicRecord", LifeRecordDbContext.SCHEMA);

            builder.Ignore(e => e.DomainEvents);

            builder.HasKey(m => m.Id);

            builder
                .Property(m => m.MusicName)
                .HasColumnName("MusicName")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(m => m.Singer)
                .HasColumnName("MusicName")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(m => m.Album)
                .HasColumnName("MusicName")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
