namespace BlazorShowcaseBackend
{
    public class LambdaEntryPointApiGw : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config.SetBasePath(Directory.GetCurrentDirectory())
                //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //        .Build();
                //})
                .UseStartup<Startup>();
        }

        /// <summary>
        /// Use this override to customize the services registered with the IHostBuilder. 
        /// 
        /// It is recommended not to call ConfigureWebHostDefaults to configure the IWebHostBuilder inside this method.
        /// Instead customize the IWebHostBuilder in the Init(IWebHostBuilder) overload.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IHostBuilder builder)
        {
        }

        protected override void PostCreateWebHost(
            IWebHost webHost)
        {
            // We want to make sure that the service is registered properly with control.
            //webHost.CheckServiceRegistration();

            // Execute the default method.
            base.PostCreateWebHost(webHost);
        }
    }
}
