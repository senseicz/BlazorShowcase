using System.Net.Http.Json;
using ShowcaseClient.Models;

namespace ShowcaseClient.Services;

public interface IUserService
{
    Task<CurrentUser> GetCurrentUserAsync();
}

public class UserService : IUserService
{
    public UserService()
    {
    }

    public async Task<CurrentUser> GetCurrentUserAsync()
    {
        return new CurrentUser()
        {
            Name = "Test Test",
            Avatar = "https://www.gravatar.com/avatar/11f26f36850d52670e910fb1aaa9f008?s=64&d=identicon&r=PG"
        };
    }
}