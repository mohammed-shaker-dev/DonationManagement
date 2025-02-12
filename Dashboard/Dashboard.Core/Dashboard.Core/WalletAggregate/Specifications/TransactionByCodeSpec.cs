using Ardalis.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.WalletAggregate.Specifications
{
    public class TransactionByCodeSpec : Specification<Wallet>
    {
        public TransactionByCodeSpec(string code)
        {
            Query
              .Include(w => w.Transactions.Where(t => t.Code.ToLower() == code));
        
        }
    }
}

 
 
