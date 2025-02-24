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
        }
    }
 
}
