using BlazorShowcase.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Serilog;
using Shared;

namespace BlazorShowcaseBackend
{
    [Authorize]
    public class ScoresDataService : ScoresData.ScoresDataBase
    {
        public ScoresDataService()
        {
        }

        public override async Task<ScoreResponse> GetScores(ScoreRequest request, ServerCallContext context)
        {
            //Log.Information("Request arrived, request {req}", JsonConvert.SerializeObject(request, Formatting.Indented));
            
            var response = new ScoreResponse { Count = 0 };

            if (request.TotalRequested > request.Downloaded)
            {
                var scoresToGenerate = request.TotalRequested - request.Downloaded;

                if (scoresToGenerate > 100)
                {
                    scoresToGenerate = 100;
                }

                var scores = new FakeDataGenerator().GenerateFakeScores(scoresToGenerate);

                response.Count = scoresToGenerate;
                response.Scores.AddRange(scores.Select(MapToTransferObject));
            }

            //Log.Information("{count} Scores sent away ({forSure})", response.Count, response.Scores.Count);

            return response;
        }

        public override async Task GetAlerts(AlertRequest request,
            IServerStreamWriter<Alert> responseStream,
            ServerCallContext context)
        {
            var scoreGenerator = new FakeDataGenerator();
            var random = new Random();
            
            while (true)
            {
                var alert = new Alert()
                {
                    CreatedOn = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
                    Id = Guid.NewGuid().ToString(),
                    Score = MapToTransferObject(scoreGenerator.GenerateFakeScores(1).First())
                };

                await responseStream.WriteAsync(alert);
                
                var randomNumberOfSeconds = random.Next(5, 15);
                Thread.Sleep(randomNumberOfSeconds * 1000);
            }
        }

        private BlazorShowcase.Data.Score MapToTransferObject(Shared.Score score)
        {
            return new BlazorShowcase.Data.Score
            {
                Id = score.Id,
                StreamId = score.StreamId,
                CreatedOn = new DateTimeOffset(score.CreatedOnUtc).ToUnixTimeMilliseconds(),
                City = score.City,
                FullName = score.FullName,
                IpAddress = score.IPAddress,
                RiskScore = score.RiskScore,
                UserName = score.UserName
            };
        }
    }
}
