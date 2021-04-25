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
    class LocationEntityTypeConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Location");

            builder.HasKey(lo=>lo.Id);//for searching

            builder.HasAlternateKey(lo => new { lo.Province, lo.City, lo.County, lo.Town, lo.Place });//ensure the location is unique.

            builder
                .Property(lo => lo.Id)
                .HasColumnName("Id")
                .IsRequired();


            builder
                .Property(lo => lo.Province)
                .HasColumnName("Province")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(lo => lo.City)
                .HasColumnName("City")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(lo => lo.County)
                .HasColumnName("County")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(lo => lo.Town)
                .HasColumnName("Town")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(lo => lo.Place)
                .HasColumnName("Place")
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
