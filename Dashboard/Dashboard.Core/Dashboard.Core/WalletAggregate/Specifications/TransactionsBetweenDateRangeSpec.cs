using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.WalletAggregate.Specifications
{
    public class TransactionsBetweenDateRangeSpec :Specification<Wallet>
    {
        public TransactionsBetweenDateRangeSpec(DateTime? fromDate, DateTime? toDate)
        {
            Query
            .Include(w => w.Transactions
                .Where(t => (fromDate == null || t.CreatedDate >= fromDate) 
                &&(toDate==null ||t.CreatedDate <= toDate)));

        }
    }
}
