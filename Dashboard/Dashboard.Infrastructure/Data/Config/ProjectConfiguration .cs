using Dashboard.Core.ProjectAggregate;
using Dashboard.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Dashboard.Infrastructure.Data.Config
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(ColumnConstants.DEFAULT_NAME_LENGTH);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.AdditionalText)
                .HasMaxLength(1000);

            builder.Property(p => p.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Planned");

            builder.Property(p => p.CreatedDate)
                .IsRequired();

            // Store images as JSON array
            builder.Property<List<string>>("_images")
                .HasColumnName("Images")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>());

            // Store videos as JSON array
            builder.Property<List<string>>("_videos")
                .HasColumnName("Videos")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>());

            // Configure one-to-many relationship with Expenses
            builder.HasMany(p => p.Expenses)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(ColumnConstants.DEFAULT_NAME_LENGTH);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.Date)
                .IsRequired();
        }
    }
}
