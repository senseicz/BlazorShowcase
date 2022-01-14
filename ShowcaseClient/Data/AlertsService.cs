using BlazorShowcase.Data;

namespace ShowcaseClient.Data
{
    public class AlertsService
    {
        private readonly ScoresData.ScoresDataClient _scoresDataClient;
        private ILogger<AlertsService> _logger;

        public static List<Alert> Alerts = new List<Alert>();

        public AlertsService(ScoresData.ScoresDataClient scoresDataClient, ILogger<AlertsService> logger)
        {
            _scoresDataClient = scoresDataClient;
            _logger = logger;
        }

        public async Task StartGettingAlerts()
        {
            _logger.LogInformation("Alerts page StartGettingAlerts START");
            using (var call = _scoresDataClient.GetAlerts(new AlertRequest()))
            {
                while (await call.ResponseStream.MoveNext(new CancellationToken()))
                {
                    Alert alert = call.ResponseStream.Current;
                    Alerts.Add(alert);
                    Alerts = Alerts.OrderByDescending(a => a.CreatedOn).ToList();
                    _logger.LogInformation("Alerts collection have {number} items now.", Alerts.Count);
                }
            }
        }
    }
}
