using Ardalis.GuardClauses;
using SharedKernel;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
