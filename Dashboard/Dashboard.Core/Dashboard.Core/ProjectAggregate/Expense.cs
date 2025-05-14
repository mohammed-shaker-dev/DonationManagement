using Ardalis.GuardClauses;
using Dashboard.Core.ValueObjects;
using SharedKernel;

namespace Dashboard.Core.ProjectAggregate
{
    public class Expense : BaseEntity<long>
    {
        private Expense() { }

        public Expense(string name, DateTime date, Money amount, string code)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Date = date;
            Amount = amount;
            Code = code;
        }

        public string Name { get; private set; }
        public DateTime Date { get; private set; }
        public Money Amount { get; private set; }
        public string Code { get; private set; }

        public void Update(string name, DateTime date, Money amount, string code)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Date = date;
            Amount = amount;
            Code = Guard.Against.NullOrWhiteSpace(code, nameof(code));
        }
    }
}
