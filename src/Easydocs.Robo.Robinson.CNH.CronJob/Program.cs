using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Easydocs.Robo.Robinson.CNH.Domain.Services;

namespace Easydocs.Robo.Robinson.CNH.CronJob
{
    class Program
    {
        protected Program() { }
        public static async Task Main()
        {
            var startUp = new Startup();

            using (var scope = startUp.Scope)
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

                logger.LogInformation("Easydocs.Robo.Robinson.CNH.Cronjob iniciado!");

                await startUp.Scope.ServiceProvider.GetService<IFindProcerssoService>().Executar();

                logger.LogInformation("Easydocs.Robo.Robinson.CNH.Cronjob finalizado!");
            }
        }
    }
}
