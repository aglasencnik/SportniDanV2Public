using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using SportniDanV2.Data;
using SportniDanV2.Models;
using System.Security.Claims;

namespace SportniDanV2.Services;

public class AuthenticatorService : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ApplicationDbContext _dataProviderService;

    public AuthenticatorService(ProtectedLocalStorage protectedLocalStorage, ApplicationDbContext dataProviderService)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _dataProviderService = dataProviderService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();

        try
        {
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>("identity");

            if (storedPrincipal.Success)
            {
                var user = JsonConvert.DeserializeObject<UserModel>(storedPrincipal.Value);
                var (_, isLookUpSuccess) = LookUpUser(user.Username, user.Password);

                if (isLookUpSuccess)
                {
                    var identity = CreateIdentityFromUser(user);
                    principal = new(identity);
                }
            }
        }
        catch
        {

        }

        return new AuthenticationState(principal);
    }

    public async Task LoginAsync(UserModel loginFormModel)
    {
        var (userInDatabase, isSuccess) = LookUpUser(loginFormModel.Username, loginFormModel.Password);
        var principal = new ClaimsPrincipal();

        if (isSuccess)
        {
            var identity = CreateIdentityFromUser(userInDatabase);
            principal = new ClaimsPrincipal(identity);
            await _protectedLocalStorage.SetAsync("identity", JsonConvert.SerializeObject(userInDatabase));
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task LogoutAsync()
    {
        await _protectedLocalStorage.DeleteAsync("identity");
        var principal = new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private ClaimsIdentity CreateIdentityFromUser(UserModel user)
    {
        return new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.Name, user.Username),
                new (ClaimTypes.Hash, user.Password),
                new (ClaimTypes.Role, user.UserType.ToString())
            }, "sportni-dan-v2");
    }

    private (UserModel, bool) LookUpUser(string username, string password)
    {
        var result = _dataProviderService.User.FirstOrDefault(u => username == u.Username && password == u.Password);

        return (result, result is not null);
    }
}
