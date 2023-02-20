using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Enums;
using System.Security.Claims;

namespace SportniDanV2.Pages.Edit;

public class EditIndexBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavManager.NavigateTo("/");
    }
}
