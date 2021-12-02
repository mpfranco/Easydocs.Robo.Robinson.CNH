using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ApplicationBuilder;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap
{
    public class ApplicationStartup
    {
        private readonly IConfiguration configuration;

        public ApplicationStartup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMetrics();

            services.AddCustomHealthChecks();

            services.AddDependencyInjection();
            

            services.AddMessages(configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCustomHealthChecks();

            app.UseMvc();
        }
    }
}