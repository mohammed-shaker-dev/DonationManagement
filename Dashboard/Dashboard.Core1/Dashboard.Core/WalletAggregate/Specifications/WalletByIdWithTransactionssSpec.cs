using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.WalletAggregate.Specifications
{
    public class WalletByIdWithTransactionssSpec : Specification<Wallet>, ISingleResultSpecification
    {
        public WalletByIdWithTransactionssSpec(long walletId)
        {
            Query
              .Where(w => w.Id == walletId)
              .Include(w => w.Transactions);
        }
    }
}
