using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SportniDanV2.Pages;

public class DashboardBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    protected IEnumerable<Claim> claims = Enumerable.Empty<Claim>();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0) NavManager.NavigateTo("/");

        claims = user.Claims;
    }
}
