using Blazored.Modal.Services;
using Blazored.Modal;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Components.EditUserComponent;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using SportniDanV2.Services;
using Syncfusion.Blazor.Grids;
using System.Security.Claims;
using Syncfusion.Blazor.Navigations;

namespace SportniDanV2.Pages.Edit;

public class UserEditBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [Inject] IToastService toast { get; set; } = null!;

    [Inject] BrowserDimensionService dimensionService { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    [CascadingParameter] public IModalService Modal { get; set; } = default!;

    protected SfGrid<UserModel> Grid { get; set; }
    protected List<UserModel> Users = new();
    protected List<UserExerciseModel> UserExercises = new();
    protected List<ItemModel> ToolbarItems = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavManager.NavigateTo("/");

        Users = db.User.ToList();
        UserExercises = db.UserExercise.ToList();

        ToolbarItems.Add(new ItemModel() { Id = "Add", Text = "Dodaj", TooltipText = "Dodaj novega uporabnika", PrefixIcon = "e-add" });
        ToolbarItems.Add(new ItemModel() { Id = "Edit", Text = "Uredi", TooltipText = "Uredi uporabnika", PrefixIcon = "e-edit" });
        ToolbarItems.Add(new ItemModel() { Id = "Delete", Text = "Izbriši", TooltipText = "Izbriši uporabnika", PrefixIcon = "e-delete" });
    }

    protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.Id == "Add")
        {
            ModalOptions options;
            var dimension = await dimensionService.GetDimensions();

            if (dimension.Width > 640)
            {
                options = new ModalOptions()
                {
                    Size = ModalSize.Medium
                };
            }
            else
            {
                options = new ModalOptions()
                {
                    Size = ModalSize.Small
                };
            }

            var parameters = new ModalParameters()
                .Add(nameof(EditUser.EditType), Enums.EditType.Add);
            var editModal = Modal.Show<EditUser>("Dodaj novega uporabnika", parameters, options);
            var result = await editModal.Result;

            if (result.Confirmed)
            {
                if (result.Data.ToString() == "true") toast.ShowSuccess("Kreiranje uporabnika je bilo USPEŠNO!");
                else toast.ShowError("Napaka pri kreiranju uporabnika!");
            }

            await Refresh();
        }
        if (args.Item.Id == "Edit")
        {
            var selectedRows = await Grid.GetSelectedRecordsAsync();

            if (selectedRows.Count != 0)
            {
                ModalOptions options;
                var dimension = await dimensionService.GetDimensions();

                if (dimension.Width > 640)
                {
                    options = new ModalOptions()
                    {
                        Size = ModalSize.Medium
                    };
                }
                else
                {
                    options = new ModalOptions()
                    {
                        Size = ModalSize.Small
                    };
                }

                var parameters = new ModalParameters()
                    .Add(nameof(EditUser.EditType), Enums.EditType.Edit)
                    .Add(nameof(EditUser.UserModel), selectedRows[0]);
                var editModal = Modal.Show<EditUser>("Uredi uporabnika", parameters, options);
                var result = await editModal.Result;

                if (result.Confirmed)
                {
                    if (result.Data.ToString() == "true") toast.ShowSuccess("Urejanje uporabnika je bilo USPEŠNO!");
                    else toast.ShowError("Napaka pri urejanju uporabnika!");
                }

                await Refresh();
            }
            else toast.ShowWarning("Noben uporabnik ni izbran!");
        }
        if (args.Item.Id == "Delete")
        {
            var selectedRows = await Grid.GetSelectedRecordsAsync();

            if (selectedRows.Count != 0)
            {
                ModalOptions options;
                var dimension = await dimensionService.GetDimensions();

                if (dimension.Width > 640)
                {
                    options = new ModalOptions()
                    {
                        Size = ModalSize.Medium
                    };
                }
                else
                {
                    options = new ModalOptions()
                    {
                        Size = ModalSize.Small
                    };
                }

                var parameters = new ModalParameters()
                    .Add(nameof(EditUser.EditType), Enums.EditType.Delete)
                    .Add(nameof(EditUser.UserModel), selectedRows[0]);
                var editModal = Modal.Show<EditUser>("Izbriši uporabnika", parameters, options);
                var result = await editModal.Result;

                if (result.Confirmed)
                {
                    if (result.Data.ToString() == "true") toast.ShowSuccess("Brisanje uporabnika je bilo USPEŠNO!");
                    else toast.ShowError("Napaka pri brisanju uporabnika!");
                }

                await Refresh();
            }
            else toast.ShowWarning("Noben uporabnik ni izbran!");
        }
    }

    protected async Task Refresh()
    {
        Users = db.User.ToList();
        UserExercises = db.UserExercise.ToList();
        await RefreshGrid();
        await RefreshGrid();
    }

    protected async Task RefreshGrid()
    {
        await Task.Delay(500);
        await Grid.Refresh();
        StateHasChanged();
    }
}
