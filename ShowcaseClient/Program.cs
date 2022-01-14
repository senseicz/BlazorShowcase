using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ShowcaseClient;
using ShowcaseClient.BFF;
using ShowcaseClient.Data;
using ShowcaseClient.Models;
using ShowcaseClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .WriteTo.BrowserConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .CreateLogger();

var options = new Options();
builder.Configuration.Bind(options);
builder.Services.AddSingleton(options);

builder.RootComponents.RegisterAsCustomElement<App>("blazor-app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// authentication state and authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddTransient<AntiforgeryHandler>();

builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)) //bffUri
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));

// gRPC-Web client with auth
builder.Services.AddShowcaseClientDataClient((services, options) =>
{
    var authEnabledHandler = services.GetRequiredService<AntiforgeryHandler>();
    authEnabledHandler.InnerHandler = new HttpClientHandler();

    options.BaseUri = builder.HostEnvironment.BaseAddress;
    options.MessageHandler = authEnabledHandler;
});

// Sets up EF Core with Sqlite
builder.Services.AddShowcaseDataDbContext();

builder.Services.AddAntDesign();
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

builder.Services.AddScoped<IUserService, UserService>();

await builder.Build().RunAsync();


