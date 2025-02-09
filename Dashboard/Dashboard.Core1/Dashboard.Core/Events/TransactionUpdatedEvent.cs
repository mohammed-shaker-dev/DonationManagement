using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Core.WalletAggregate;
using SharedKernel;

namespace Dashboard.Core.Events
{
    public class TransactionUpdatedEvent:BaseDomainEvent
    {
        public TransactionUpdatedEvent(Transaction transaction)
        {
               TransactionUpdated = transaction;
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Transaction TransactionUpdated { get; set; }
    }
}
