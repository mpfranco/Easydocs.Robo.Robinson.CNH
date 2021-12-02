using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Easydocs.Robo.Robinson.CNH.Application.UseCases.Services;
using Easydocs.Robo.Robinson.CNH.Domain.Services;
using System;
using RestSharp;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class PoliciesHttpClientServiceCollectionExtension
    {
        public static void AddPolicies(this IServiceCollection services)
        {
            services.AddScoped<RestClient>();
            //services.AddHttpClient<IFindInvoiceService, FindInvoiceService>()
            //   .AddPolicyHandler(x =>
            //   {
            //       return HttpPolicyExtensions
            //           .HandleTransientHttpError()
            //           .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1));
            //   });
        }
    }
}
