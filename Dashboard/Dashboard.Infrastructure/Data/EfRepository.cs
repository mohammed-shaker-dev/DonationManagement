using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
namespace Dashboard.Infrastructure.Data
{
    public class EfRepository<T>: RepositoryBase<T>,IRepository<T> where T : class,IAggregateRoot
    {
        public EfRepository(AppDbContext dbContext):base(dbContext) 
        {
            
        }
    }
}
