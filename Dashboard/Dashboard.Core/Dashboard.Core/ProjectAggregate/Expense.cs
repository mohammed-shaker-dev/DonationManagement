using Ardalis.GuardClauses;
using SharedKernel;

namespace Dashboard.Core.ProjectAggregate
{
    public class Expense : BaseEntity<long>
    {
        private Expense() { }

        public Expense(string name, DateTime date, decimal value, string code)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Date = date;
            Value = value;
            Code = Guard.Against.NullOrWhiteSpace(code, nameof(code));
        }

        public string Name { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Value { get; private set; }
        public string Code { get; private set; }

        public void Update(string name, DateTime date, decimal value, string code)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Date = date;
            Value = value;
            Code = Guard.Against.NullOrWhiteSpace(code, nameof(code));
        }
    }
}
