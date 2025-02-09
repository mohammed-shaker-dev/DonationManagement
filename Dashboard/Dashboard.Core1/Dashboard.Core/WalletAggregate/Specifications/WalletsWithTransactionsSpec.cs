using Ardalis.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.WalletAggregate.Specifications
{
    public class WalletsWithTransactionsSpec : Specification<Wallet>
    {
        public WalletsWithTransactionsSpec( )
        {
            Query
              .Include(w => w.Transactions);  
        }
    }
}
