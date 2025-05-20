using SharedKernel.Enums;

namespace SharedKernel.Blazor.Shared
{
    public class UpdateProjectRequest
    {
        public string Status { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public ProjectType? ProjectType { get; set; }
    }
}
