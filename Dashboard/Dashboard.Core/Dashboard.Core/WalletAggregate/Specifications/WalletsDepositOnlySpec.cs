using Ardalis.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.WalletAggregate.Specifications
{
    public class WalletsDepositOnlySpec : Specification<Wallet>
    {
        public WalletsDepositOnlySpec( )
        {
            Query
              .Include(w => w.Transactions.Where(t => t.TransactionType == SharedKernel.Enums.TransactionType.IN));
        }
    }
 
}
