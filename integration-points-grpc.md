# gRPC integration

## Shared project

* protobuf.proto

        syntax = "proto3";

        option csharp_namespace = "BlazorShowcase.Data";

        service ScoresData {
            rpc GetScores(ScoreRequest) returns (ScoreResponse) {}
        }

        message ScoreRequest {
            int32 totalRequested = 1;
            int32 downloaded = 2;
        }

        message ScoreResponse {
            repeated Score scores = 1;
            int32 count = 2;
        }

        message Score {
            string id = 1;
            string streamId = 2;
            int64 createdOn = 3;
        }

## Server side

* .csproj file

        <ItemGroup>
            <PackageReference Include="Grpc.AspNetCore" Version="2.40.0" />
            <PackageReference Include="Grpc.AspNetCore.Web" Version="2.40.0" />

            <ProjectReference Include="..\Shared\Shared.csproj" />

            <Protobuf Include="..\Shared\protobuf.proto" GrpcServices="Server" />
        </ItemGroup>

* "Data" service 

        public class ScoresDataService : ScoresData.ScoresDataBase
        {
            public ScoresDataService() {/* DI stuff here */ }

            public override async Task<ScoreResponse> GetScores(ScoreRequest request, ServerCallContext context)
            {
                var response = new ScoreResponse { Count = 0 };
                var scores = new FakeDataGenerator().GenerateFakeScores(100);
                response.Count = 100;
                response.Scores.AddRange(scores.Select(MapToTransferObject));
                return response;
            }
        }

## Client side

* .csproj file

        <PackageReference Include="Google.Protobuf" Version="3.18.1" />
        <PackageReference Include="Grpc.Net.Client" Version="2.41.0" />
        <PackageReference Include="Grpc.Net.Client.Web" Version="2.41.0" />
        <PackageReference Include="Grpc.Tools" Version="2.43.0">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        
        <Protobuf Include="..\Shared\protobuf.proto" GrpcServices="Client" />

* Startup.cs 

        // gRPC-Web client with auth
        builder.Services.AddShowcaseClientDataClient((services, options) =>
        {
            var authEnabledHandler = services.GetRequiredService<AntiforgeryHandler>();
            authEnabledHandler.InnerHandler = new HttpClientHandler();

            options.BaseUri = builder.HostEnvironment.BaseAddress;
            options.MessageHandler = authEnabledHandler;
        });

* ShowcaseClientDataServices.cs (extension method called from Startup)

            public static void AddShowcaseClientDataClient(this IServiceCollection serviceCollection, Action<IServiceProvider, AddShowcaseClientDataClientOptions> configure)
            {
                serviceCollection.AddScoped(services =>
                {
                    var options = new AddShowcaseClientDataClientOptions();
                    configure(services, options);
                    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, options.MessageHandler!));
                    var channel = GrpcChannel.ForAddress(options.BaseUri!, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, ThrowOperationCanceledOnCancellation = false});
                    return new BlazorShowcase.Data.ScoresData.ScoresDataClient(channel);
                });
            }

### Now you have ScoresData.ScoresDataClient in DI container and can use it anywhere you want

    [Inject] private ScoresData.ScoresDataClient _scoredDataClient { get; set; }
    private List<Score> _scores = new List<Score>();

    public async Task GetData()
    {
        var response = await _scoredDataClient.GetScoresAsync(new ScoreRequest() { Downloaded = 0, TotalRequested = 20 });
        _scores = response.Scores.ToList();
    }

