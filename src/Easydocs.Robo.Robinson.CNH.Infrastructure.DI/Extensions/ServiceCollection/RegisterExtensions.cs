using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Robinson.CNH.Application.UseCases.Services;
using Easydocs.Robo.Robinson.CNH.Domain.Services;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Services;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class RegisterExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IFindProcerssoService, FindProcessoService>();
            services.AddScoped<ILoggerRomaneio, LoggerRomaneio>();  
            return services;
        }
    }
}