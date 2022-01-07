using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace BlazorShowcaseBackend;

public class Program
{
    public static void Main(string[] args)
    {
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
                



                
                
                webBuilder.UseStartup<Startup>();
            });
}


