using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Comunication;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.DomainObjects;
using Easydocs.Robo.Robinson.CNH.Application.UseCases.Commands.Romaneio.AddRomaneio;
using Easydocs.Robo.Robinson.CNH.Application.UseCases.Commands.Romaneio.UpdateRomaneio;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class MessageBusServiceCollectionExtensions
    {
        public static void AddBus(this IServiceCollection services)
        {
            //  Bus (Mediator)
            services.AddMediatR(typeof(ApplicationStartup));
            services.AddScoped<IMediatorBus, MediatorBus>();
            services.AddScoped<IRequestHandler<AddRomaneioCommand, ResultCommand>, AddRomaneioCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateRomaneioCommand, ResultCommand>, UpdateRomaneioCommandHandler>();

        }
    }
}
