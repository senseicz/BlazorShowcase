using BlazorShowcase.Data;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;

//var channel = GrpcChannel.ForAddress("https://fraud-test-api.cibfrauddev.aws.dsarena.com/");

var channel = GrpcChannel.ForAddress("https://localhost:7777/");
var client = new BlazorShowcase.Data.ScoresData.ScoresDataClient(channel);


var scores = await client.GetScoresAsync(new ScoreRequest());

foreach (var score in scores.Scores)
{
    Console.WriteLine(JsonConvert.SerializeObject(score));
}

Console.ReadLine();
