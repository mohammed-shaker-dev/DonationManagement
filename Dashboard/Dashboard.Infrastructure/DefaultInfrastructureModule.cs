using Dashboard.Core.Interfaces;
using Dashboard.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure
{
    public static class DefaultInfrastructureModule
    {
        public static void AddInfrastructureDependencies(this IServiceCollection services, bool isDevelopemnt)
        {
            if(isDevelopemnt)
            {
                RegisterDevelopmentOnlyDependencies(services);
            }
            else
            {
                RegisterProductionOnlyDependencies(services);
            }
            RegisterCommonDependencies(services);
        }

        private static void RegisterCommonDependencies(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>),typeof(EfRepository<>));
            services.AddScoped(typeof(EfRepository<>));
            services.AddScoped(typeof(IEmailSender), typeof(EmailSender));
            services.AddScoped<AppDbContextSeed>();

        }
        private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
        {

        }
        private static void RegisterProductionOnlyDependencies(IServiceCollection services)
        {

        }
    }
}
