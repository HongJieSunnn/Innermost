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
    class LifeRecordEntityTypeConfiguration
        : IEntityTypeConfiguration<LifeRecord>
    {
        public void Configure(EntityTypeBuilder<LifeRecord> builder)
        {
            builder.ToTable("LifeRecord");

            builder.Ignore(e => e.DomainEvents);

            builder.Ignore(e => e.EmotionTags);

            builder.HasKey(l => l.Id);

            builder
                .Property(l => l.Title)
                .HasColumnName("Title")
                .HasMaxLength(200)
                .IsRequired(false);

            builder
                .Property(l => l.Text)
                .HasColumnName("Text")
                .HasMaxLength(3000)
                .IsRequired();

            builder
                .OwnsOne(l => l.Location, loc =>
                {
                    loc.Property<int>("LifeRecordId");
                    loc.HasKey("LifeRecordId");
                    loc.WithOwner().HasForeignKey("LifeRecordId");
                });

            builder
                .Property<int>("_musicRecordId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("MusicRecordId")
                .IsRequired();

            builder
                .Property(l => l.Path)
                .HasDefaultValue("/记录时刻")
                .HasColumnName("Path")
                .IsRequired();

            builder
                .Property("_isShared")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("IsShared")
                .IsRequired();

            builder
                .Property<int>("_textTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("TextTypeId")
                .IsRequired();
                

            builder
                .HasOne(l=>l.TextType)
                .WithMany()
                .HasForeignKey("_textTypeId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(l=>l.MusicRecord)
                .WithMany()
                .HasForeignKey("_musicRecordId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
