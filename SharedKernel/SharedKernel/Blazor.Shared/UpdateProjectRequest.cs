using SharedKernel.Enums;

namespace SharedKernel.Blazor.Shared
{
    public class UpdateProjectRequest
    {
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AdditionalText { get; set; }
        public ProjectStatus Status { get; set; }
        public ProjectType? ProjectType { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Videos { get; set; } = new List<string>();
    }
}