using Ardalis.Specification;
using Dashboard.Core.ProjectAggregate;
using SharedKernel.Enums;

namespace Dashboard.Core.ProjectAggregate.Specifications
{
    public class ProjectsByTypeSpec : Specification<Project>
    {
        public ProjectsByTypeSpec(ProjectType projectType)
        {
            Query
                .Where(p => p.ProjectType == projectType)
                .Include(p => p.Expenses);
        }
    }
}