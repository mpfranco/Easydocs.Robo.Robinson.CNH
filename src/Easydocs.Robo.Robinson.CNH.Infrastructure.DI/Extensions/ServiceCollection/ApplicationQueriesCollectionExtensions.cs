using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Robinson.CNH.Application.UseCases.Queries.Occurrences;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class ApplicationQueriesCollectionExtensions
    {
        public static void AddApplicationQueries(this IServiceCollection services)
        {
            services.AddScoped<IFindInvoiceQuerie, FindInvoiceQuerie>();            
        }
    }
}
