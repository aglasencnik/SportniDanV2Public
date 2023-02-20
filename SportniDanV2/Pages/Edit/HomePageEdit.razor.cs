using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using SportniDanV2.Services;
using Syncfusion.Blazor.RichTextEditor;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SportniDanV2.Pages.Edit;

public class HomePageEditBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [Inject] IToastService toast { get; set; } = null!;

    [Inject] IConfiguration configuration { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    protected string RTEValue { get; set; } = "";

    protected AppDataModel appData = new();

    protected List<ToolbarItemModel> Tools = new()
    {
        new ToolbarItemModel() { Command = ToolbarCommand.Bold },
        new ToolbarItemModel() { Command = ToolbarCommand.Italic },
        new ToolbarItemModel() { Command = ToolbarCommand.Underline },
        new ToolbarItemModel() { Command = ToolbarCommand.StrikeThrough },
        new ToolbarItemModel() { Command = ToolbarCommand.FontName },
        new ToolbarItemModel() { Command = ToolbarCommand.FontSize },
        new ToolbarItemModel() { Command = ToolbarCommand.FontColor },
        new ToolbarItemModel() { Command = ToolbarCommand.BackgroundColor },
        new ToolbarItemModel() { Command = ToolbarCommand.LowerCase },
        new ToolbarItemModel() { Command = ToolbarCommand.UpperCase },
        new ToolbarItemModel() { Command = ToolbarCommand.SuperScript },
        new ToolbarItemModel() { Command = ToolbarCommand.SubScript },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.Formats },
        new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
        new ToolbarItemModel() { Command = ToolbarCommand.NumberFormatList },
        new ToolbarItemModel() { Command = ToolbarCommand.BulletFormatList },
        new ToolbarItemModel() { Command = ToolbarCommand.Outdent },
        new ToolbarItemModel() { Command = ToolbarCommand.Indent },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.CreateLink },
        new ToolbarItemModel() { Command = ToolbarCommand.Image },
        new ToolbarItemModel() { Command = ToolbarCommand.CreateTable },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.ClearFormat },
        new ToolbarItemModel() { Command = ToolbarCommand.Print },
        new ToolbarItemModel() { Command = ToolbarCommand.SourceCode },
        new ToolbarItemModel() { Command = ToolbarCommand.FullScreen },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.Undo },
        new ToolbarItemModel() { Command = ToolbarCommand.Redo }
    };
    protected List<TableToolbarItemModel> TableQuickToolbarItems = new()
    {
        new TableToolbarItemModel() { Command = TableToolbarCommand.TableHeader },
        new TableToolbarItemModel() { Command = TableToolbarCommand.TableRows },
        new TableToolbarItemModel() { Command = TableToolbarCommand.TableColumns },
        new TableToolbarItemModel() { Command = TableToolbarCommand.TableCell },
        new TableToolbarItemModel() { Command = TableToolbarCommand.HorizontalSeparator },
        new TableToolbarItemModel() { Command = TableToolbarCommand.TableRemove },
        new TableToolbarItemModel() { Command = TableToolbarCommand.BackgroundColor },
        new TableToolbarItemModel() { Command = TableToolbarCommand.TableCellVerticalAlign },
        new TableToolbarItemModel() { Command = TableToolbarCommand.Styles }
    };

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavManager.NavigateTo("/");

        appData = db.AppData.FirstOrDefault(x => x.DataType == Enums.DataType.HomePageHtml);

        if (appData == null) RTEValue = configuration["DefaultHomePage"];
        else RTEValue = appData.Value;
    }

    protected async Task SaveHomePage()
    {
        try
        {
            if (appData != null) appData.Value = RTEValue;
            else await db.AppData.AddAsync(new AppDataModel() { DataType = Enums.DataType.HomePageHtml, Value = RTEValue });

            await db.SaveChangesAsync();

            toast.ShowSuccess("Shranjevanje uspešno!");
        }
        catch
        {
            toast.ShowError("Napaka pri shranjevanju!");
        }
    }
}
