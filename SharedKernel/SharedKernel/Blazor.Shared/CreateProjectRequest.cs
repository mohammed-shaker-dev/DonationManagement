using SharedKernel.Enums;

namespace SharedKernel.Blazor.Shared
{
    public class CreateProjectRequest
    {
        public DateTime StartedDate { get; set; }=DateTime.Now;
        public DateTime CompletedDate { get; set; } = DateTime.Now;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string AdditionalText { get; set; } = "";
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Videos { get; set; } = new List<string>();
        public List<CreateExpenseRequest> Expenses { get; set; } = new List<CreateExpenseRequest>();
        public string WalletName { get; set; } = "SYP";
        public ProjectType ProjectType { get; set; } = ProjectType.Donation;

    }
}
