using BlazorShowcase.Data;
using Grpc.Core;
using Shared;

namespace BlazorShowcaseBackend
{
    public class ScoresDataService : ScoresData.ScoresDataBase
    {
        public ScoresDataService()
        {
        }

        public override async Task<ScoreResponse> GetScores(ScoreRequest request, ServerCallContext context)
        {
            var scores = new FakeDataGenerator().GenerateFakeScores(100);

            var response = new ScoreResponse { Count = scores.Count };
            response.Scores.AddRange(scores.Select(MapToTransferObject));

            return response;
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
