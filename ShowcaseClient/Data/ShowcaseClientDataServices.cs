using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.EntityFrameworkCore;

namespace ShowcaseClient.Data;

public static class ShowcaseClientDataServices
{
    public static void AddShowcaseClientDataClient(this IServiceCollection serviceCollection, Action<IServiceProvider, AddShowcaseClientDataClientOptions> configure)
    {
        serviceCollection.AddScoped(services =>
        {
            var options = new AddShowcaseClientDataClientOptions();
            configure(services, options);
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, options.MessageHandler!));
            var channel = GrpcChannel.ForAddress(options.BaseUri!, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null });
            return new BlazorShowcase.Data.AlertsData.AlertsDataClient(channel);
        });
    }

    public static void AddShowcaseDataDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContextFactory<ClientSideDbContext>(
            options => options.UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}"));
        serviceCollection.AddScoped<DataSynchronizer>();
    }
}

public class AddShowcaseClientDataClientOptions
{
    public string? BaseUri { get; set; }
    public HttpMessageHandler? MessageHandler { get; set; }
}
