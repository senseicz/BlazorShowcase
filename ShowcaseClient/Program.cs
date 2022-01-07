using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShowcaseClient;
using ShowcaseClient.BFF;
using ShowcaseClient.Data;
using ShowcaseClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
var bffUri = builder.Configuration["BffUri"]!;
builder.RootComponents.RegisterAsCustomElement<App>("blazor-app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// authentication state and authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddTransient<AntiforgeryHandler>();

//builder.Services
//    .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(_onePassClientName))
//    .AddHttpClient(_onePassClientName, client => client.BaseAddress = new Uri(onePassUri))
//    .AddHttpMessageHandler<AuthorizationMessageHandler>();




builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri(bffUri))
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));




// gRPC-Web client with auth
builder.Services.AddShowcaseClientDataClient((services, options) =>
{
    var authEnabledHandler = services.GetRequiredService<AntiforgeryHandler>();
    //authEnabledHandler.ConfigureHandler(new[] { bffUri });
    //authEnabledHandler.InnerHandler = new HttpClientHandler();

    options.BaseUri = bffUri;
    options.MessageHandler = authEnabledHandler;
});

// Supplies an IAuthorizationStateProvider service that lets other components know about auth state
// This one gets that state by asking the OpenID Connect client. Also we cache the state for offline use.
builder.Services.AddApiAuthorization(c =>
{
    c.ProviderOptions.ConfigurationEndpoint = $"{bffUri}/client-configuration/showcase-client";
});
builder.Services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, OfflineAccountClaimsPrincipalFactory>();

// Sets up EF Core with Sqlite
builder.Services.AddShowcaseDataDbContext();

builder.Services.AddAntDesign();
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();


