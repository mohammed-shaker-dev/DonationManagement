using Dashboard.Core.WalletAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure.Data.Config
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(p => p.Id);
            builder.Property(p=>p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Code)
           .HasMaxLength(10)
           .IsRequired();

            builder.Property(t => t.Code)
                .HasDefaultValueSql("FORMAT(NEXT VALUE FOR dbo.TransactionCodeSequence, '0000000000')");
        }
    }
}
