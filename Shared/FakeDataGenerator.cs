using Bogus;

namespace Shared
{
    public class FakeDataGenerator
    {
        private List<string> _streamIds = new List<string>();

        public List<Score> GenerateFakeScores(int count)
        {
            for (int i = 0; i < 100; i++)
            {
                _streamIds.Add(Guid.NewGuid().ToString());
            }

            var fakeScores = new Faker<Score>("en")
                .StrictMode(true)
                .RuleFor(o => o.Id, f => f.Random.Guid().ToString())
                .RuleFor(o => o.StreamId, f => f.PickRandom(_streamIds))
                .RuleFor(o => o.CreatedOnUtc, f => f.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now))
                .RuleFor(o => o.UserName, f => f.Internet.UserName())
                .RuleFor(o => o.FullName, f => f.Name.FullName())
                .RuleFor(o => o.IPAddress, f => f.Internet.Ip())
                .RuleFor(o => o.City, f => f.Address.City())
                .RuleFor(o => o.RiskScore, f => f.Random.Int(0, 1000));

            return fakeScores.Generate(count);
        }
    }
}