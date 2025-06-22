// Updated Project.cs
using Ardalis.GuardClauses;
using Dashboard.Core.DTOs;
using Dashboard.Core.ValueObjects;
using SharedKernel;
using SharedKernel.Interfaces;
using Dashboard.Core.WalletAggregate;
using System.Text.Json.Serialization;
using SharedKernel.Enums;

namespace Dashboard.Core.ProjectAggregate
{
    public class Project : BaseEntity<long>, IAggregateRoot
    {
        private Project() { }

        public Project(string name, string description, string additionalText, DateTime createdDate, DateTime completedDate, ProjectType projectType = ProjectType.Donation,ProjectStatus projectStatus=ProjectStatus.Completed)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Description = description;
            AdditionalText = additionalText;
            CreatedDate = createdDate;
            CompletedDate = completedDate;
            ProjectType = projectType;
            Status = projectStatus;
            _images = new List<string>();
            _videos = new List<string>();
        }


        public string Name { get; private set; }
        public string Description { get; private set; }
        public string AdditionalText { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        public ProjectType ProjectType { get; private set; } = ProjectType.Donation;
        public ProjectStatus Status { get; private set; } = ProjectStatus.Planned;

        [JsonIgnore]
        private readonly List<Expense> _expenses = new List<Expense>();
        public IEnumerable<Expense> Expenses => _expenses.AsReadOnly();

        [JsonIgnore]
        private readonly List<string> _images;
        public IEnumerable<string> Images => _images.AsReadOnly();

        [JsonIgnore]
        private readonly List<string> _videos;
        public IEnumerable<string> Videos => _videos.AsReadOnly();

        public Dictionary<string, decimal> GetCurrencyTotals()
        {
            return _expenses
                .GroupBy(e => e.Amount.Currency.Code)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));
        }

        // Add method to update dates
        public void UpdateDates(DateTime? startedDate, DateTime? completedDate)
        {
            if (startedDate.HasValue)
            {
                CreatedDate = startedDate.Value;
            }

            CompletedDate = completedDate;

            // If completed date is set, update status to Completed
            if (completedDate.HasValue)
            {
                Status = ProjectStatus.Completed;
            }
        }
        public void AddExpense(Expense expense)
        {
            Guard.Against.Null(expense, nameof(expense));
            _expenses.Add(expense);
        }

        public void UpdateExpense(long expenseId, string name, DateTime date, Money amount, string code)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == expenseId);
            Guard.Against.Null(expense, nameof(expense));
            expense.Update(name, date, amount, code);
        }

        public void DeleteExpense(long expenseId)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == expenseId);
            Guard.Against.Null(expense, nameof(expense));
            _expenses.Remove(expense);
        }
        public void UpdateProjectType(ProjectType projectType)
        {
            ProjectType = projectType;
        }
        public void AddImage(string imageUrl)
        {
            Guard.Against.NullOrWhiteSpace(imageUrl, nameof(imageUrl));
            _images.Add(imageUrl);
        }

        public void RemoveImage(string imageUrl)
        {
            _images.Remove(imageUrl);
        }

        public void AddVideo(string videoUrl)
        {
            Guard.Against.NullOrWhiteSpace(videoUrl, nameof(videoUrl));
            _videos.Add(videoUrl);
        }

        public void RemoveVideo(string videoUrl)
        {
            _videos.Remove(videoUrl);
        }

        public void UpdateStatus(ProjectStatus status)
        {
            Status = status;

            if (status == ProjectStatus.Completed && !CompletedDate.HasValue)
            {
                CompletedDate = DateTime.UtcNow;
            }
        }
 
        public void UpdateBasicInfo(string name, string description, string additionalText)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Description = description ?? Description;
            AdditionalText = additionalText ?? AdditionalText;
        }
        public ProjectDTO ToDto()
        {
            return new ProjectDTO
            {
                Id = Id,
                Name = Name,
                Description = Description,
                AdditionalText = AdditionalText,
                Status = Status,
                CreatedDate = CreatedDate,
                CompletedDate = CompletedDate,
                CurrencyTotals = GetCurrencyTotals(),
                Images = Images.ToList(),
                Videos = Videos.ToList(),
                ProjectType = ProjectType,
                Expenses = Expenses.Select(e => new ExpenseDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Date = e.Date,
                    Value = e.Amount.Amount,
                    Code = e.Code,
                    CurrencyCode = e.Amount.Currency.Code,
                }).ToList()
            };
        }
    }
}
