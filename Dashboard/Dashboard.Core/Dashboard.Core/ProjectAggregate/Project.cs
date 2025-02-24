using Ardalis.GuardClauses;
using SharedKernel;
using SharedKernel.Interfaces;

namespace Dashboard.Core.ProjectAggregate
{
    public class Project : BaseEntity<long>, IAggregateRoot
    {
        private Project() { }

        public Project(string name, string description, string additionalText)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Description = description;
            AdditionalText = additionalText;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string AdditionalText { get; private set; }

        private readonly List<Expense> _expenses = new List<Expense>();
        public IEnumerable<Expense> Expenses => _expenses.AsReadOnly();

        public void AddExpense(Expense expense)
        {
            Guard.Against.Null(expense, nameof(expense));
            _expenses.Add(expense);
        }

        public void UpdateExpense(long expenseId, string name, DateTime date, decimal value, string code)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == expenseId);
            Guard.Against.Null(expense, nameof(expense));
            expense.Update(name, date, value, code);
        }

        public void DeleteExpense(long expenseId)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == expenseId);
            Guard.Against.Null(expense, nameof(expense));
            _expenses.Remove(expense);
        }
    }
}