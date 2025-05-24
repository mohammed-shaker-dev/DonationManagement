using Dashboard.Core.ProjectAggregate;
using Dashboard.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Enums;
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
 

            builder.Property(p => p.CreatedDate)
                .IsRequired();

            builder.Property(p => p.Status)
               .HasConversion(
                   v => ConvertProjectStatusToString(v),
                   v => ConvertStringToProjectStatus(v))
               .HasMaxLength(20);

            builder.Property(p => p.ProjectType)
                .HasConversion(
                    v => ConvertProjectTypeToString(v),
                    v => ConvertStringToProjectType(v))
                .HasMaxLength(20);
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
        private static string ConvertProjectStatusToString(ProjectStatus status)
        {
            return status switch
            {
                ProjectStatus.Planned => "Planned",
                ProjectStatus.InProgress => "InProgress",
                ProjectStatus.Completed => "Completed",
                _ => "Planned"
            };
        }

        private static ProjectStatus ConvertStringToProjectStatus(string status)
        {
            return status switch
            {
                "Planned" => ProjectStatus.Planned,
                "InProgress" => ProjectStatus.InProgress,
                "Completed" => ProjectStatus.Completed,
                "1" => ProjectStatus.Planned,
                "2" => ProjectStatus.InProgress,
                "3" => ProjectStatus.Completed,
                _ => ProjectStatus.Planned
            };
        }

        private static string ConvertProjectTypeToString(ProjectType type)
        {
            return type switch
            {
                ProjectType.Donation => "Donation",
                ProjectType.Organization => "Organization",
                _ => "Donation"
            };
        }

        private static ProjectType ConvertStringToProjectType(string type)
        {
            return type switch
            {
                "Donation" => ProjectType.Donation,
                "Organization" => ProjectType.Organization,
                "1" => ProjectType.Donation,
                "2" => ProjectType.Organization,
                _ => ProjectType.Donation
            };
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

            // Configure Money as an owned entity
            builder.OwnsOne(e => e.Amount, a =>
            {
                a.Property(p => p.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                // Configure the nested Currency owned entity
                a.OwnsOne(m => m.Currency, c =>
                {
                    c.Property(p => p.Code).HasColumnName("CurrencyCode");
                });
            });

            builder.Property(e => e.Date)
                .IsRequired();
        }
    }
}
