using Dashboard.Core.WalletAggregate;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.Events
{
    public class TransactionDeletedEvent : BaseDomainEvent
    {
        public TransactionDeletedEvent(Transaction transaction)
        {
            TransactionDeleted = transaction;
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Transaction TransactionDeleted { get; set; }
    }
}
