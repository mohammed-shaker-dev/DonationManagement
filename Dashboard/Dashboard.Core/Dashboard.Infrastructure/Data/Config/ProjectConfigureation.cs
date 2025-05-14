using Dashboard.Core.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure.Data.Config
{
    public class ProjectConfigureation : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasMany(p => p.Expenses)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(p => p.Images, a =>
            {
                a.WithOwner().HasForeignKey("ProjectId");
                a.Property<long>("Id");
                a.HasKey("Id");
                a.Property(i => i.Url).IsRequired();
                a.Property(i => i.Description);
            });

            builder.OwnsMany(p => p.Videos, a =>
            {
                a.WithOwner().HasForeignKey("ProjectId");
                a.Property<long>("Id");
                a.HasKey("Id");
                a.Property(v => v.Url).IsRequired();
                a.Property(v => v.Description);
            });
        }
    }
}

