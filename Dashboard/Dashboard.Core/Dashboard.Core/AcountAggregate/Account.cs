using SharedKernel;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.BudgetAggregate
{
    public class Account: BaseEntity<long>,IAggregateRoot
    {
    }
}
