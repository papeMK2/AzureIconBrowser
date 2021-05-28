using AzureIconBrowser.Backend.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIconBrowser.Backend
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();
            var services = builder.Services;
            var configuration = context.Configuration;

            services.Configure<IconStorageOptions>(configuration.GetSection("IconStorage"));

        }
    }
}
