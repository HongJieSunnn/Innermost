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
    class TextTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<TextType>
    {
        public void Configure(EntityTypeBuilder<TextType> builder)
        {
            builder.ToTable("TextType");

            builder.HasKey(t =>t.Id);

            builder
                .Property(t => t.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder
                .Property(t => t.Name)
                .HasColumnName("TextTypeName")
                .IsRequired();
        }
    }
}
