using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Settings;


namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class SettingsServiceCollectionExtensions
    {
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            //Settings
            services.Configure<RoboRobinsonSettings>(configuration.GetSection("Robo.Solumax"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<RoboRobinsonSettings>>().Value);
            
        }
    }
}
