using Blazored.Modal.Services;
using Blazored.Modal;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Components.EditExerciseComponent;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using SportniDanV2.Services;
using Syncfusion.Blazor.Grids;
using System.Security.Claims;
using Syncfusion.Blazor.Navigations;

namespace SportniDanV2.Pages.Edit;

public class ExerciseEditBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [Inject] IToastService toast { get; set; } = null!;

    [Inject] BrowserDimensionService dimensionService { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    [CascadingParameter] public IModalService Modal { get; set; } = default!;

    protected SfGrid<ExerciseModel> Grid { get; set; }
    protected List<ExerciseModel> Exercises = new();
    protected List<ItemModel> ToolbarItems = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavManager.NavigateTo("/");

        Exercises = db.Exercise.ToList();

        ToolbarItems.Add(new ItemModel() { Id = "Add", Text = "Dodaj", TooltipText = "Dodaj novo nalogo", PrefixIcon = "e-add" });
        ToolbarItems.Add(new ItemModel() { Id = "Edit", Text = "Uredi", TooltipText = "Uredi nalogo", PrefixIcon = "e-edit" });
        ToolbarItems.Add(new ItemModel() { Id = "Delete", Text = "Izbriši", TooltipText = "Izbriši nalogo", PrefixIcon = "e-delete" });
    }

    protected async Task ToolbarClickHandler(ClickEventArgs args)
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
                .Add(nameof(EditExercise.EditType), Enums.EditType.Add);
            var editModal = Modal.Show<EditExercise>("Dodaj novo nalogo", parameters, options);
            var result = await editModal.Result;

            if (result.Confirmed)
            {
                if (result.Data.ToString() == "true") toast.ShowSuccess("Kreiranje naloge je bilo USPEŠNO!");
                else toast.ShowError("Napaka pri kreiranju naloge!");
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
                    .Add(nameof(EditExercise.EditType), Enums.EditType.Edit)
                    .Add(nameof(EditExercise.ExerciseModel), selectedRows[0]);
                var editModal = Modal.Show<EditExercise>("Uredi nalogo", parameters, options);
                var result = await editModal.Result;

                if (result.Confirmed)
                {
                    if (result.Data.ToString() == "true") toast.ShowSuccess("Urejanje naloge je bilo USPEŠNO!");
                    else toast.ShowError("Napaka pri urejanju naloge!");
                }

                await Refresh();
            }
            else toast.ShowWarning("Nobena naloga ni izbrana!");
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
                    .Add(nameof(EditExercise.EditType), Enums.EditType.Delete)
                    .Add(nameof(EditExercise.ExerciseModel), selectedRows[0]);
                var editModal = Modal.Show<EditExercise>("Izbriši nalogo", parameters, options);
                var result = await editModal.Result;

                if (result.Confirmed)
                {
                    if (result.Data.ToString() == "true") toast.ShowSuccess("Brisanje naloge je bilo USPEŠNO!");
                    else toast.ShowError("Napaka pri brisanju naloge!");
                }

                await Refresh();
            }
            else toast.ShowWarning("Nobena naloga ni izbrana!");
        }
    }

    protected async Task Refresh()
    {
        Exercises = db.Exercise.ToList();
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
