using Ardalis.GuardClauses;
using Dashboard.Core.ValueObjects;
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
            _images = new List<Image>();
            _videos = new List<Video>();
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string AdditionalText { get; private set; }

        private readonly List<Expense> _expenses = new List<Expense>();
        public IEnumerable<Expense> Expenses => _expenses.AsReadOnly();

        private readonly List<Image> _images;
        public IEnumerable<Image> Images => _images.AsReadOnly();

        private readonly List<Video> _videos;
        public IEnumerable<Video> Videos => _videos.AsReadOnly();

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

        public void AddImage(Image image)
        {
            Guard.Against.Null(image, nameof(image));
            _images.Add(image);
        }

        public void RemoveImage(Image image)
        {
            Guard.Against.Null(image, nameof(image));
            _images.Remove(image);
        }

        public void AddVideo(Video video)
        {
            Guard.Against.Null(video, nameof(video));
            _videos.Add(video);
        }

        public void RemoveVideo(Video video)
        {
            Guard.Against.Null(video, nameof(video));
            _videos.Remove(video);
        }

        public ProjectDTO ToDTO()
        {
            return new ProjectDTO
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                AdditionalText = this.AdditionalText,
                Images = this.Images.Select(i => new ImageDTO { Url = i.Url, Description = i.Description }).ToList(),
                Videos = this.Videos.Select(v => new VideoDTO { Url = v.Url, Description = v.Description }).ToList()
            };
        }
    }
}