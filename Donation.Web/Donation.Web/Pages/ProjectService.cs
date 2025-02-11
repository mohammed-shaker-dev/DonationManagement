namespace Donation.Web.Pages
{
    public class ProjectService
    {
        private List<Project> projects = new()
    {
        new Project { Name = "School Renovation", Description = "Renovated a local school.", Cost = 5000, ImageUrl = "images/school.jpg" },
        new Project { Name = "Water Well", Description = "Built a water well in a rural area.", Cost = 3000, ImageUrl = "images/well.jpg" }
    };

        public List<Project> GetProjects()
        {
            return projects;
        }
    }

    public class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string ImageUrl { get; set; }
    }
}
