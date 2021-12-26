using AntDesign.ProLayout;

namespace ShowcaseClient.Layouts;

public partial class BasicLayout
{
    
    private MenuDataItem[] _menuData = Array.Empty<MenuDataItem>();


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var dashboardChildrens = new List<MenuDataItem>
        {
            new MenuDataItem() { Path = "/", Name = "Scores", Key = "dashboard.scores" },
            new MenuDataItem()
                { Path = "/dashboard/alerts", Name = "Alerts", Key = "dashboard.alerts", Icon = "alert" },
        };

        var menu = new List<MenuDataItem>
        {
            new MenuDataItem()
            {
                Path = "/dashboard", Name = "Dashboard", Icon = "dashboard", Key = "dashboard",
                Children = dashboardChildrens.ToArray()
            }
        };
        
        _menuData = menu.ToArray();
    }
   
}