using Ardalis.GuardClauses;
using Dashboard.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.Guards
{
    public static class MoneyCustomGuard
    {
        public static void MismatchCurrency(this IGuardClause guardClause, string currency1, string currency2)
        {
            if (!currency1.Equals(currency2, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Currencies do not match.");
            }
        }
        public static void NegativeValue(this IGuardClause guardClause,Money money)
        {
            if (money.Amount<=0)
            {
                throw new ArgumentException("Currencies do not match.");
            }
        }
    }
}
