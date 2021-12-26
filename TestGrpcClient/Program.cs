using BlazorShowcase.Data;
using Grpc.Net.Client;
using Newtonsoft.Json;

// for testing against AWS-deployed lambda
//var channel = GrpcChannel.ForAddress("https://fraud-test-api.cibfrauddev.aws.dsarena.com/");


//for local testing, make sure BlazorShowcaseBackend is running.
var channel = GrpcChannel.ForAddress("https://localhost:7777/");

var client = new BlazorShowcase.Data.ScoresData.ScoresDataClient(channel);


var scores = await client.GetScoresAsync(new ScoreRequest());

foreach (var score in scores.Scores)
{
    Console.WriteLine(JsonConvert.SerializeObject(score));
}

Console.ReadLine();
