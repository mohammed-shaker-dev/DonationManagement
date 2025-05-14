// Step 1: Create a new specification in Dashboard.Core.ProjectAggregate.Specifications
using Ardalis.Specification;
using Dashboard.Core.ProjectAggregate;

namespace Dashboard.Core.ProjectAggregate.Specifications
{
    public class ProjectsWithExpensesSpec : Specification<Project>
    {
        public ProjectsWithExpensesSpec()
        {
            Query.Include(p => p.Expenses);
        }
    }
    public class ProjectWithExpensesSpec : Specification<Project>, ISingleResultSpecification
    {
        public ProjectWithExpensesSpec(long projectId)
        {
            Query
                .Where(p => p.Id == projectId)
                .Include(p => p.Expenses);
        }
    }
}