using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.WalletAggregate.Specifications
{
    public class WalletByNameWithTransactionssSpec : Specification<Wallet>, ISingleResultSpecification
    {
        public WalletByNameWithTransactionssSpec(string walletName)
        {
            Query
              .Where(w => w.Name.ToLower() == walletName.ToLower())
              .Include(w => w.Transactions);
        }
    }
}
