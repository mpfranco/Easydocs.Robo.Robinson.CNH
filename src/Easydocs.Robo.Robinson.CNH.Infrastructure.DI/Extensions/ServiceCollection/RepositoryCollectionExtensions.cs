﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Robinson.CNH.Domain.Interfaces.IRepository.Invoices;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Data.Repository.Invoices;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class RepositoryCollectionExtensions
    {
        public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStringSuperConciliacao =
                configuration.GetSection("ConnectionString:RoboVazFiel").Value;

            services.AddScoped<IFindInvoice, RepositoryRomaneio>();
            
        }
    }
}
