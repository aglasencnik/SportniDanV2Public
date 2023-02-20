using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Models;
using System.ComponentModel.DataAnnotations;
using System;
using SportniDanV2.Services;
using SportniDanV2.Data;

namespace SportniDanV2.Pages.Account;

public class LoginBase : ComponentBase
{
    [Inject] AuthenticatorService AuthService { get; set; } = null!;

    [Inject] NavigationManager NavManager { get; set; } = null!;

    [Inject] ApplicationDbContext AppDbContext { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    protected LoginModel loginModel { get; set; } = new();
    protected string errorMessage = "";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() > 0) NavManager.NavigateTo("/dashboard/");
    }

    protected async Task TryLogin()
    {
        if (loginModel.Username != "" && loginModel.Password != "")
        {
            errorMessage = "";
            UserModel userModel = new UserModel()
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            };
            var search = AppDbContext.User.FirstOrDefault(x => x.Username == userModel.Username);

            if (search != null)
            {
                if (search.Password == loginModel.Password)
                {
                    await AuthService.LoginAsync(userModel);
                    NavManager.NavigateTo("/dashboard/");
                }
                else errorMessage = "Geslo ni pravilno!";
            }
            else errorMessage = "Uporabnik ni bil najden!";
        }
        else errorMessage = "Eno ali več polj za prijavo ni bilo izpolnjenih!";
    }

    protected class LoginModel
    {
        [Required(ErrorMessage = "Uporabniško ime je obvezno!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno!")]
        public string Password { get; set; }
    }
}
