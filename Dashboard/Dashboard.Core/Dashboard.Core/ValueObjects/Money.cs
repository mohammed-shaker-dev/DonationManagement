﻿using Ardalis.GuardClauses;
using Dashboard.Core.Guards;
using SharedKernel;
using System.Text.Json.Serialization;

namespace Dashboard.Core.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; }
        private Money() { }
        public Money(decimal amount )
        {
            Guard.Against.Negative(amount, nameof(amount));
            Amount = amount;
            Currency = new Currency("SYP");
        }
        [JsonConstructor]
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
