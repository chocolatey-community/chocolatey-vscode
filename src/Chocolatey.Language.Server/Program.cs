using System;
using System.Linq;
using System.Threading.Tasks;
using Chocolatey.Language.Server.Handlers;
using Chocolatey.Language.Server.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Chocolatey.Language.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new LanguageServerOptions()
                .WithInput(Console.OpenStandardInput())
                .WithOutput(Console.OpenStandardOutput())
                .WithLoggerFactory(new LoggerFactory())
                .AddDefaultLoggingProvider()
                .WithMinimumLogLevel(LogLevel.Trace)
                .WithServices(ConfigureServices)
                .WithHandler<TextDocumentSyncHandler>()
                .WithHandler<CodeActionHandler>()
                .WithHandler<ConfigurationHandler>()
                .OnInitialize((s, _) => {
                    var serviceProvider = (s as LanguageServer).Services;
                    var bufferManager = serviceProvider.GetService<BufferManager>();
                    var diagnosticsHandler = serviceProvider.GetService<DiagnosticsHandler>();

                    // Hook up diagnostics
                    bufferManager.BufferUpdated += (__, x) => diagnosticsHandler.PublishDiagnostics(x.Uri, bufferManager.GetBuffer(x.Uri));

                    return Task.CompletedTask;
                });

            var server = await LanguageServer.From(options);

            await server.WaitForExit;
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BufferManager>();
            services.AddSingleton<DiagnosticsHandler>();
            services.AddSingleton<Configuration>();
            services.AddSingleton<INuSpecRule, UrlValidValidation>();
            services.AddSingleton<INuSpecRule, DescriptionLengthValidation>();
            services.AddSingleton<INuSpecRule, DoesNotContainTemplatedValues>();
            services.AddTransient<IConfigurationProvider>(config => config.GetServices<IJsonRpcHandler>().OfType<ConfigurationHandler>().FirstOrDefault());
        }
    }
}
