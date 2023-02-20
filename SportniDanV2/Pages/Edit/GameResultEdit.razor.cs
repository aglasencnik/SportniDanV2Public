using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using SportniDanV2.Services;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System.Security.Claims;

namespace SportniDanV2.Pages.Edit;

public class GameResultEditBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [Inject] IToastService toast { get; set; } = null!;

    [Inject] BrowserDimensionService dimensionService { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    [CascadingParameter] public IModalService Modal { get; set; } = default!;

    protected SfGrid<GameResultModel> Grid { get; set; }
    protected List<GameResultModel> GameResults = new();
    protected List<ClassModel> Classes = new();
    protected List<ExerciseModel> Exercises = new();
    protected List<ItemModel> ToolbarItems = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavManager.NavigateTo("/");

        GameResults = db.GameResult.ToList();
        Classes = db.Class.ToList();
        Exercises = db.Exercise.ToList();

        ToolbarItems.Add(new ItemModel() { Id = "Add", Text = "Dodaj", TooltipText = "Dodaj nov rezultat igre", PrefixIcon = "e-add" });
        ToolbarItems.Add(new ItemModel() { Id = "Edit", Text = "Uredi", TooltipText = "Uredi rezultat igre", PrefixIcon = "e-edit" });
        ToolbarItems.Add(new ItemModel() { Id = "Delete", Text = "Izbriši", TooltipText = "Izbriši rezultat igre", PrefixIcon = "e-delete" });
    }

    protected async Task ToolbarClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Add")
        {
            NavManager.NavigateTo("/insertresults");
        }
        if (args.Item.Id == "Edit")
        {
            var selectedRows = await Grid.GetSelectedRecordsAsync();

            if (selectedRows.Count != 0) NavManager.NavigateTo($"/insertresults/{selectedRows[0].ExerciseId}");
            else toast.ShowWarning("Noben rezultat igre ni izbran!");
        }
        if (args.Item.Id == "Delete")
        {
            var selectedRows = await Grid.GetSelectedRecordsAsync();

            if (selectedRows.Count != 0)
            {
                try
                {
                    db.Remove(selectedRows[0]);
                    await db.SaveChangesAsync();

                    toast.ShowSuccess("Brisanje uspešno!");
                    await Refresh();
                }
                catch
                {
                    toast.ShowError("Napaka pri brisanju!");
                }
            }
            else toast.ShowWarning("Noben rezultat igre ni izbran!");
        }
    }

    protected async Task Refresh()
    {
        GameResults = db.GameResult.ToList();
        Classes = db.Class.ToList();
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
