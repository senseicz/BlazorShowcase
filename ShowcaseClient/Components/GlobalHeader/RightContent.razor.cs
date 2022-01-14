using AntDesign;
using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components;
using ShowcaseClient.Models;
using ShowcaseClient.Services;

namespace ShowcaseClient.Components
{
    public partial class RightContent
    {
        private CurrentUser _currentUser = new CurrentUser();
        private int _count = 0;

        private string FullUserName = "";

        public AvatarMenuItem[] AvatarMenuItems { get; set; } = new AvatarMenuItem[]
        {
            new() { Key = "setting", IconType = "setting", Option = "Settings"},
            new() { IsDivider = true },
            new() { Key = "logout", IconType = "logout", Option = "Logout"}
        };

        [Inject] protected NavigationManager NavigationManager { get; set; }

        [Inject] protected IUserService UserService { get; set; }
        

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            SetClassMap();

            _currentUser = await UserService.GetCurrentUserAsync();
        }

        protected void SetClassMap()
        {
            ClassMapper
                .Clear()
                .Add("right");
        }

        public void HandleSelectUser(MenuItem item)
        {
            switch (item.Key)
            {
                case "setting":
                    NavigationManager.NavigateTo("/");
                    break;
                case "logout":
                    NavigationManager.NavigateTo("https://localhost:7777" + _currentUser.LogoutUri);
                    break;
            }
        }
    }
}