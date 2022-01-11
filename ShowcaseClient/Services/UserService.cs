using Microsoft.AspNetCore.Components.Authorization;
using ShowcaseClient.Models;

namespace ShowcaseClient.Services;

public interface IUserService
{
    Task<CurrentUser> GetCurrentUserAsync();
}

public class UserService : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;


    public UserService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<CurrentUser> GetCurrentUserAsync()
    {
        var user = new CurrentUser();
        var authUser = await _authenticationStateProvider.GetAuthenticationStateAsync();


        if (authUser.User.Identity != null)
        {
            var givenNameClaim = authUser.User.FindFirst("given_name");
            var familyNameClaim = authUser.User.FindFirst("family_name");

            string fullUserName = "";
            
            if (givenNameClaim != null)
            {
                fullUserName += givenNameClaim.Value;
            }

            if (familyNameClaim != null)
            {
                fullUserName += " " + familyNameClaim.Value;
            }

            user.Name = fullUserName;
            user.Avatar = "https://www.gravatar.com/avatar/11f26f36850d52670e910fb1aaa9f008?s=64&d=identicon&r=PG";

            var logoutUriClaim = authUser.User.FindFirst("bff:logout_url");
            if (logoutUriClaim != null)
            {
                user.LogoutUri = logoutUriClaim.Value;
            }

            var preferredUsernameClaim = authUser.User.FindFirst("preferred_username");
            if (preferredUsernameClaim != null)
            {
                user.PreferredUsername = preferredUsernameClaim.Value;
            }
        }

        return user;
    }
}