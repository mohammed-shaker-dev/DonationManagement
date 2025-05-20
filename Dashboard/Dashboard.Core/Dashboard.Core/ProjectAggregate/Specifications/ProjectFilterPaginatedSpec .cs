using Ardalis.Specification;
using Dashboard.Core.ProjectAggregate;
using SharedKernel.Enums;

namespace Dashboard.Core.ProjectAggregate.Specifications
{
    public class ProjectFilterPaginatedSpec : Specification<Project>
    {
        public ProjectFilterPaginatedSpec(ProjectType? projectType = null, int? skip = null, int? take = null)
        {
            // Include expenses for all projects
            Query.Include(p => p.Expenses);

            // Filter by project type if specified
            if (projectType.HasValue)
            {
                Query.Where(p => p.ProjectType == projectType.Value);
            }

            // Apply pagination if specified
            if (skip.HasValue)
            {
                Query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                Query.Take(take.Value);
            }

            // Order by creation date (newest first)
            Query.OrderByDescending(p => p.CreatedDate);
        }
    }

    public class ProjectCountSpec : Specification<Project>
    {
        public ProjectCountSpec(ProjectType? projectType = null)
        {
            // Filter by project type for counting
            if (projectType.HasValue)
            {
                Query.Where(p => p.ProjectType == projectType.Value);
            }
        }
    }
}