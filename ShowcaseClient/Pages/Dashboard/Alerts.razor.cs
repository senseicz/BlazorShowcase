using BlazorShowcase.Data;
using Microsoft.AspNetCore.Components;

namespace ShowcaseClient.Pages.Dashboard;

public partial class Alerts
{
    [Inject] private ScoresData.ScoresDataClient _scoresDataClient { get; set; }

    [Inject] private ILogger<Alerts> _logger { get; set; }

    private List<Alert> _alerts = new List<Alert>();

    protected override async Task OnInitializedAsync()
    {
        _logger.LogInformation("Alerts page - OnItializedAsync START");
        await StartGettingAlerts();

    }

    public async Task StartGettingAlerts()
    {
        _logger.LogInformation("Alerts page StartGettingAlerts START");
        using (var call = _scoresDataClient.GetAlerts(new AlertRequest()))
        {
            while (await call.ResponseStream.MoveNext(new CancellationToken()))
            {
                Alert alert = call.ResponseStream.Current;
                _alerts.Add(alert);
                _alerts = _alerts.OrderByDescending(a => a.CreatedOn).ToList();
                _logger.LogInformation("Alerts collection have {number} items now.", _alerts.Count);

                StateHasChanged();
            }
        }
    }
}