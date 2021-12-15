using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShowcaseClient;
using ShowcaseClient.Data;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
var backendOrigin = builder.Configuration["BackendOrigin"]!;
builder.RootComponents.RegisterAsCustomElement<App>("blazor-app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services
    .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazeOrbital.CentralServerAPI"))
    .AddHttpClient("BlazeOrbital.CentralServerAPI", client => client.BaseAddress = new Uri(backendOrigin))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// gRPC-Web client with auth
builder.Services.AddShowcaseClientDataClient((services, options) =>
{
    var authEnabledHandler = services.GetRequiredService<AuthorizationMessageHandler>();
    authEnabledHandler.ConfigureHandler(new[] { backendOrigin });
    authEnabledHandler.InnerHandler = new HttpClientHandler();

    options.BaseUri = backendOrigin;
    options.MessageHandler = authEnabledHandler;
});

// Supplies an IAuthorizationStateProvider service that lets other components know about auth state
// This one gets that state by asking the OpenID Connect client. Also we cache the state for offline use.
builder.Services.AddApiAuthorization(c =>
{
    c.ProviderOptions.ConfigurationEndpoint = $"{backendOrigin}/_configuration/BlazeOrbital.ManufacturingHub";
});
builder.Services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, OfflineAccountClaimsPrincipalFactory>();

// Sets up EF Core with Sqlite
builder.Services.AddShowcaseDataDbContext();





builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();