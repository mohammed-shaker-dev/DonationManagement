using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure.Data
{
    public static class IdentityHelpers
    {
        public static bool IsRealDatabase(this DbContext context)
        {
            return context.Database.ProviderName.Contains("SqlServer");
        }
    }
}
