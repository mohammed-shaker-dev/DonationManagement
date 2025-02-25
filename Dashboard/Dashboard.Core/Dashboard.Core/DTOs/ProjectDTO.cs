namespace Dashboard.Core.DTOs
{
    public class ProjectDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdditionalText { get; set; }
        public List<ImageDTO> Images { get; set; }
        public List<VideoDTO> Videos { get; set; }
    }

    public class ImageDTO
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }

    public class VideoDTO
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }
}
