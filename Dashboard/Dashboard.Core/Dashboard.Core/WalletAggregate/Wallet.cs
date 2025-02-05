using Ardalis.GuardClauses;
using Dashboard.Core.Events;
using Dashboard.Core.ValueObjects;
using SharedKernel;
using SharedKernel.Interfaces;

namespace Dashboard.Core.WalletAggregate
{
    public class Wallet: BaseEntity<long>,IAggregateRoot
    {
        public Wallet(string name, Currency currency)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Currency = currency;
        }
        public string Name { get; private set; }
        public Currency Currency { get; private set; }

        private readonly List<Transaction> _transactions = new List<Transaction>();
        public IEnumerable<Transaction> Transactions => _transactions.AsReadOnly();

        public void AddNewTransaction(Transaction transaction)
        {
            Guard.Against.Null(transaction, nameof(transaction));

            _transactions.Add(transaction);


            var transactionCreated = new TransactionCreatedEvent(transaction);
            Events.Add(transactionCreated);
        }
        public Money GetAmmount()
        {
            var inAmount = _transactions.Where(t=>t.TransactionType==SharedKernel.Enums.TransactionType.IN).Sum(t => t.Amount);
            var outAmount = _transactions.Where(t=>t.TransactionType==SharedKernel.Enums.TransactionType.OUT).Sum(t => t.Amount);
            return new Money(inAmount-outAmount,Currency);
        }

    }
}
