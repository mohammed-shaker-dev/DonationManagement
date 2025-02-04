using SharedKernel;
using Ardalis.GuardClauses;
using System.Globalization;

namespace Dashboard.Core.ValueObjects
{
    public class Currency : ValueObject
    {
        public static readonly Currency USD = new("USD");
        public static readonly Currency EUR = new("EUR");
        public static readonly Currency SYP = new("SYP");
        public string Code { get; }
        private Currency(string code)
        {
            Guard.Against.NullOrWhiteSpace(code, nameof(code));
            code=code.ToUpper(CultureInfo.InvariantCulture);
        }
        public static Currency FomCode(string code)
        {
            return code.ToUpper(CultureInfo.InvariantCulture) switch
            {
                "USD"=>USD,
                "EUR"=>EUR,
                "SYP"=>SYP,
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
