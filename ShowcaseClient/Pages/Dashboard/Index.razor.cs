using BlazorShowcase.Data;
using Microsoft.AspNetCore.Components;


namespace ShowcaseClient.Pages.Dashboard;

public partial class Index
{

    [Inject] private ScoresData.ScoresDataClient _scoredDataClient { get; set; }

    private List<Score> _scores = new List<Score>();

    public async Task GetData()
    {
        var response = await _scoredDataClient.GetScoresAsync(new ScoreRequest() { Downloaded = 0, TotalRequested = 20 });
        _scores = response.Scores.ToList();
    }





}