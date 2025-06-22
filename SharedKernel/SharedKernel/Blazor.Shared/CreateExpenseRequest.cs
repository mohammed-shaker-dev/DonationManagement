namespace SharedKernel.Blazor.Shared
{
    public class CreateExpenseRequest
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
        public string CurrencyCode { get; set; } = "SYP";
        public bool CreateTransaction { get; set; } = true;
    }
}
