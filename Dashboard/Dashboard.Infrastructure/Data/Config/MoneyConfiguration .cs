//using Dashboard.Core.ValueObjects;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Dashboard.Infrastructure.Data.Config
//{
//    public class MoneyConfiguration : IEntityTypeConfiguration<Money>
//    {
//        public void Configure(EntityTypeBuilder<Money> builder)
//        {
//            builder.HasNoKey();
//            builder.Property(m => m.Amount);
//            builder.OwnsOne(m => m.Currency);
//        }
//    }
//}
