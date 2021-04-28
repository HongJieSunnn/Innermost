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
    class LifeRecordEntityTypeConfiguration
        : IEntityTypeConfiguration<LifeRecord>
    {
        public void Configure(EntityTypeBuilder<LifeRecord> builder)
        {
            builder.ToTable("LifeRecord");

            builder.Ignore(e => e.DomainEvents);

            builder.Ignore(e => e.EmotionTags);

            builder.HasKey(l => l.Id);

            //TODO add indexs to the columns always be searched.
            builder.HasIndex(l => l.Path);

            builder.HasIndex(l => l.PublishTime);

            builder
                .Property("_userId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UserId")
                .HasMaxLength(95)
                .IsRequired();

            builder
                .Property(l => l.Title)
                .HasColumnName("Title")
                .HasMaxLength(200)
                .IsRequired(false);

            builder
                .Property(l => l.Text)
                .HasCharSet(CharSet.Utf8Mb4)
                .HasColumnName("Text")
                .HasMaxLength(3000)
                .IsRequired();

            builder
                .Property("_locationId")
                .HasColumnName("LocationId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .IsRequired();

            builder
                .Property(l => l.PublishTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("PublishTime")
                .IsRequired();

            builder
                .Property<int>("_musicRecordId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("MusicRecordId")
                .IsRequired();

            builder
                .Property(l => l.Path)
                .HasDefaultValue("Memories")
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
                .HasOne(l => l.Location)
                .WithMany()
                .HasForeignKey("_locationId")
                .OnDelete(DeleteBehavior.NoAction)
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
