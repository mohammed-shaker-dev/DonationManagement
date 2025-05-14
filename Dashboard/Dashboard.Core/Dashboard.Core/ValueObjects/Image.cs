using Ardalis.GuardClauses;
using SharedKernel;

namespace Dashboard.Core.ValueObjects
{
    public class Image : ValueObject
    {
        public string Url { get; private set; }
        public string Description { get; private set; }

        public Image(string url, string description)
        {
            Url = Guard.Against.NullOrWhiteSpace(url, nameof(url));
            Description = description;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            yield return Description;
        }
    }
}
