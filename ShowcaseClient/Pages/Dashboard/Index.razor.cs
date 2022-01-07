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


    [Parameter] public string? SearchName { get; set; }

    string[] categories = Array.Empty<string>();
    string[] subcategories = Array.Empty<string>();
    string searchName = string.Empty;
    int minStock, maxStock = 50000;

    IQueryable<Score>? GetFilteredScores()
    {
        if (db is null)
        {
            return null;
        }

        var result = db.Scores.AsNoTracking().AsQueryable();
        
        
        /*
        
        if (categories.Any())
        {
            result = result.Where(x => categories.Contains(x.Category));
        }
        if (subcategories.Any())
        {
            result = result.Where(x => subcategories.Contains(x.Subcategory));
        }
        if (!string.IsNullOrEmpty(searchName))
        {
            result = result.Where(x => EF.Functions.Like(x.Name, searchName.Replace("%", "\\%") + "%", "\\"));
        }
        if (minStock > 0)
        {
            result = result.Where(x => x.Stock >= minStock);
        }
        if (maxStock < 50000)
        {
            result = result.Where(x => x.Stock <= maxStock);
        }
        */

        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        db = await _dataSynchronizer.GetPreparedDbContextAsync();
        _dataSynchronizer.OnUpdate += StateHasChanged;
        _total = _dataSynchronizer.SyncTotal;
    }

    protected override void OnParametersSet()
    {
        searchName = SearchName ?? string.Empty;
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