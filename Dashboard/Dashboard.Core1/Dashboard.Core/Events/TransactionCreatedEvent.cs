
using Dashboard.Core.WalletAggregate;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.Events
{
    public class TransactionCreatedEvent :BaseDomainEvent
    {
        public TransactionCreatedEvent(Transaction transaction)
        {
            TransactionCreated = transaction;
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Transaction TransactionCreated { get; set; }
    }
}
