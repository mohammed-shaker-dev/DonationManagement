using Ardalis.GuardClauses;
using SharedKernel;
using SharedKernel.Interfaces;

namespace Dashboard.Core.BudgetAggregate
{
    public class Wallet: BaseEntity<long>,IAggregateRoot
    {
        public Wallet(string name)
        {
            Name = Guard.Against.NullOrWhiteSpace(name,nameof(name));
        }
        public string Name { get; private set; }

        private readonly List<Transaction> _transactions = new List<Transaction>();
        public IEnumerable<Transaction> Transactions => _transactions.AsReadOnly();
   
    }
}
