using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure.EntityConfigurations
{
    class EmotionTagEntityTypeConfiguration
        : IEntityTypeConfiguration<EmotionTag>
    {
        public void Configure(EntityTypeBuilder<EmotionTag> builder)
        {
            builder.ToTable("EmotionTag");

            builder.HasKey(e => new { e.Id, e.LifeRecordId });

            builder
                .Property(e => e.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnName("EmotionName")
                .IsRequired();

            builder
                .Property(e => e.EmotionEmoji)
                .HasCharSet(CharSet.Utf8Mb4)
                .IsRequired();
        }
    }
}
