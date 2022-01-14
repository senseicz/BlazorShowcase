using BlazorShowcase.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Components;

namespace ShowcaseClient.Pages.Dashboard;

public partial class Alerts : IDisposable
{
    [Inject] private ScoresData.ScoresDataClient _scoresDataClient { get; set; }

    [Inject] private ILogger<Alerts> _logger { get; set; }

    private List<Alert> _alerts = new List<Alert>();

    private AsyncServerStreamingCall<Alert> _call;

    protected override async Task OnInitializedAsync()
    {
        _logger.LogInformation("Alerts page - OnItializedAsync START");

        //I've tried multiple approaches how to handle server streaming call cancellation, none seems to work
        //without throwing an exception at the very end. Thus this "hack" approach.
        try
        {
            await StartGettingAlerts();
        }
        catch (Grpc.Core.RpcException ex)
        {
            _logger.LogInformation("gRPC exception caught and handled gracefully.", ex);
        }

    }

    public async Task StartGettingAlerts()
    {
        _logger.LogInformation("Alerts page StartGettingAlerts START");

        _call = _scoresDataClient.GetAlerts(new AlertRequest());

        await foreach (var alert in _call.ResponseStream.ReadAllAsync())
        {
            _alerts.Add(alert);
            _alerts = _alerts.OrderByDescending(a => a.CreatedOn).ToList();
            _logger.LogInformation("Alerts collection have {number} items now.", _alerts.Count);

            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("Alerts page - Dispose START");
        _call.Dispose();
    }
}