using AntDesign.ProLayout;

namespace ShowcaseClient.Layouts
{

    public partial class BasicLayout
    {

        private MenuDataItem[] _menuData = Array.Empty<MenuDataItem>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var dashboardChildrens = new List<MenuDataItem>
            {
                new MenuDataItem() { Path = "/", Name = "Home", Key = "dashboard.home", Icon="icon-c-sharp-l"  },
                new MenuDataItem() { Path = "/scores", Name = "Scores", Key = "dashboard.scores", Icon="desktop" },
                new MenuDataItem() { Path = "/alerts", Name = "Alerts", Key = "dashboard.alerts", Icon = "alert" },
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
}