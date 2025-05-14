using Ardalis.Specification;
using Dashboard.Core.ProjectAggregate;

namespace Dashboard.Core.ProjectAggregate.Specifications
{
    public class ProjectByIdWithExpensesSpec : Specification<Project>, ISingleResultSpecification
    {
        public ProjectByIdWithExpensesSpec(long projectId)
        {
            Query
                .Where(p => p.Id == projectId)
                .Include(p => p.Expenses);
        }
    }
}