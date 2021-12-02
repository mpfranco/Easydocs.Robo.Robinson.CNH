﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class HealthChecksServiceCollectionExtensions
    {
        public static void AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();
        }
    }
}
