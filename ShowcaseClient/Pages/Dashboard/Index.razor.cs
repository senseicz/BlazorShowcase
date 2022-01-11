using AntDesign;
using BlazorShowcase.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using ShowcaseClient.Data;

namespace ShowcaseClient.Pages.Dashboard;

public partial class Index
{
    ClientSideDbContext? db;
    ITable table;
    int _total = 0;

    private List<Score> scores;


    [Parameter] public string? SearchUserName { get; set; }
    [Parameter] public string? SearchFullName { get; set; }

    [Inject] private ScoresData.ScoresDataClient _scoredDataClient { get; set; }

    string searchUserName = string.Empty;
    string searchFullName = string.Empty;
    int minScore, maxScore = 1000;

    IQueryable<Score>? GetFilteredScores()
    {
        if (db is null)
        {
            return null;
        }

        var result = db.Scores.AsNoTracking().AsQueryable();
        
        if (!string.IsNullOrEmpty(searchUserName))
        {
            result = result.Where(x => EF.Functions.Like(x.UserName, searchUserName.Replace("%", "\\%") + "%", "\\"));
        }

        if (!string.IsNullOrEmpty(searchFullName))
        {
            result = result.Where(x => EF.Functions.Like(x.FullName, searchFullName.Replace("%", "\\%") + "%", "\\"));
        }

        if (minScore > 0)
        {
            result = result.Where(x => x.RiskScore >= minScore);
        }
        if (maxScore < 1000)
        {
            result = result.Where(x => x.RiskScore <= maxScore);
        }
        

        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        db = await _dataSynchronizer.GetPreparedDbContextAsync();
        _dataSynchronizer.OnUpdate += StateHasChanged;
        _total = _dataSynchronizer.SyncTotal;


        //var data = await _scoredDataClient.GetScoresAsync(new ScoreRequest());
        //scores = data.Scores.ToList();
    }

    protected override void OnParametersSet()
    {
        searchFullName = SearchFullName ?? string.Empty;
        searchUserName = SearchUserName ?? string.Empty;
    }

    public void Dispose()
    {
        db?.Dispose();
        _dataSynchronizer.OnUpdate -= StateHasChanged;
    }


    private RenderFragment Info(string title, string value, bool bordered = false)
    {

        return new RenderFragment(b =>
        {
            b.OpenElement(0, "div");
            b.AddAttribute(1, "class", "headerInfo");

            b.OpenElement(2, "span");
            b.AddContent(3, title);
            b.CloseElement(); //span

            b.OpenElement(4, "p");
            b.AddContent(5, value);
            b.CloseElement(); //p
            
            if (bordered)
            {
                b.OpenElement(6, "em");
                b.CloseElement();
            }
            
            b.CloseElement(); //div
        });

    }







}