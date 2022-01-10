using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace BlazorShowcaseBackend;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information("Starting up");


        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
        {
            CreateHostBuilder(args).Build().Run();
        }
        else
        {
            //this is for ALB handling

            //var lambdaEntry = new LambdaEntryPointAlb();
            //var functionHandler =
            //    (Func<ApplicationLoadBalancerRequest, ILambdaContext, Task<ApplicationLoadBalancerResponse>>)
            //    (lambdaEntry.FunctionHandlerAsync);

            //this is for ApiGateway handling

            var lambdaEntry = new LambdaEntryPointApiGw();
            var functionHandler =
                (Func<APIGatewayProxyRequest, ILambdaContext, Task<APIGatewayProxyResponse>>)
                (lambdaEntry.FunctionHandlerAsync);

            using var handlerWrapper = HandlerWrapper.GetHandlerWrapper(functionHandler, new DefaultLambdaJsonSerializer());
            using var bootstrap = new LambdaBootstrap(handlerWrapper);

            bootstrap.RunAsync().Wait();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseSerilog((ctx, lc) => lc
                    .MinimumLevel.Warning()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .MinimumLevel.Override("IdentityModel", LogEventLevel.Debug)
                    .MinimumLevel.Override("Duende.Bff", LogEventLevel.Debug)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                        theme: AnsiConsoleTheme.Code));

                Log.Information("STARTUP!!!!");

                webBuilder.UseStartup<Startup>();
            });
}


