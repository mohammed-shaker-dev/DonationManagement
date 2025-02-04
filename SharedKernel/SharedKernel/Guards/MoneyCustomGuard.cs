using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Guards
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
    }
}
