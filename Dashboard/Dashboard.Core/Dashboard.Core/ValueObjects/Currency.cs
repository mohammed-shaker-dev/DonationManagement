using Ardalis.GuardClauses;
using SharedKernel;

using System.Globalization;
using System.Text.Json.Serialization;

namespace Dashboard.Core.ValueObjects
{
    public class Currency : ValueObject
    {
        public string Code { get; }
        private Currency()
        {
                
        }
        [JsonConstructor]
        public Currency(string code)
        {
            Guard.Against.NullOrWhiteSpace(code, nameof(code));
            Code = code.ToUpper(CultureInfo.InvariantCulture);
        }
        public static Currency FomCode(string code)
        {
            return code.ToUpper(CultureInfo.InvariantCulture) switch
            {
                "USD" => new("USD"),
                "EUR"=> new("EUR"),
                "SYP"=> new("SYP"),
                _=> throw new ArgumentException(nameof(code), nameof(code))
            };
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
        public override string ToString() => Code;
    }
}
