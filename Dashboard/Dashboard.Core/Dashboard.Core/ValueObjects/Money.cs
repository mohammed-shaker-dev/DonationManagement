using Ardalis.GuardClauses;
using SharedKernel;
using SharedKernel.Guards;

namespace Dashboard.Core.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public Money(decimal amount,Currency currency)
        {
            Guard.Against.Negative(amount,nameof(amount));
            Guard.Against.Null(currency,nameof(currency));
            Amount = amount;
            Currency = currency;
        }
        public Money Add(Money other)
        {
            Guard.Against.Null(other, nameof(other));
            Guard.Against.MismatchCurrency(Currency.Code, other.Currency.Code);

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            Guard.Against.Null(other, nameof(other));
            Guard.Against.MismatchCurrency(Currency.Code, other.Currency.Code);

            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal multiplier)
        {
            return new Money(Amount * multiplier, Currency);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
           yield return Amount;
           yield return Currency;
        }
        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}
