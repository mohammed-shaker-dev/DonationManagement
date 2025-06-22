using System;

namespace SharedKernel.Blazor.Shared
{
    public class UpdateExpenseRequest
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
        public string CurrencyCode { get; set; } // Must match existing expense's currency
    }
}
