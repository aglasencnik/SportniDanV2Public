using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using Syncfusion.Blazor.RichTextEditor;
using System.ComponentModel.DataAnnotations;

namespace SportniDanV2.Components.EditExerciseComponent;

public class EditExerciseBase : ComponentBase
{
    [Inject] IToastService toast { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter] public Enums.EditType EditType { get; set; }

    [Parameter] public ExerciseModel? ExerciseModel { get; set; } = new();

    protected InputModel inputModel = new();

    protected string errorMessage = "";

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

    protected override void OnInitialized()
    {
        if (ExerciseModel != null)
        {
            inputModel.Id = ExerciseModel.Id;
            inputModel.ExerciseNumber = ExerciseModel.ExerciseNumber;
            inputModel.Title = ExerciseModel.Title;
            inputModel.Location = ExerciseModel.Location;
            inputModel.Rules = ExerciseModel.Rules;
            inputModel.Props = ExerciseModel.Props;
            inputModel.ExerciseType = ExerciseModel.ExerciseType;
            inputModel.OrderType = ExerciseModel.OrderType;
            inputModel.NumberOfPlayers = ExerciseModel.NumberOfPlayers;
        }
    }

    protected async Task FormSubmit()
    {
        if (inputModel.ExerciseNumber != 0 &&
            inputModel.Title != "" &&
            inputModel.Location != "" &&
            inputModel.Rules != "" &&
            inputModel.Props != "" &&
            inputModel.NumberOfPlayers != 0)
        {
            if (EditType == Enums.EditType.Add) await CreateItem();
            else if (EditType == Enums.EditType.Edit) await EditItem();
            else await DeleteItem();
        }
        else errorMessage = "Nezadostni vnosi podatkov!";
    }

    protected async Task CloseModalOk() => await BlazoredModal.CloseAsync(ModalResult.Ok<string>("true"));

    protected async Task CloseModalErr() => await BlazoredModal.CloseAsync(ModalResult.Ok<string>("false"));

    protected async Task CreateItem()
    {
        try
        {
            var model = new ExerciseModel()
            {
                ExerciseNumber = inputModel.ExerciseNumber,
                Title = inputModel.Title,
                Location = inputModel.Location,
                Rules = inputModel.Rules,
                Props = inputModel.Props,
                ExerciseType = inputModel.ExerciseType,
                OrderType = inputModel.OrderType,
                NumberOfPlayers = inputModel.NumberOfPlayers
            };

            await db.Exercise.AddAsync(model);
            await db.SaveChangesAsync();

            await CloseModalOk();
        }
        catch
        {
            await CloseModalErr();
        }
    }

    protected async Task EditItem()
    {
        try
        {
            var item = db.Exercise.FirstOrDefault(x => x.Id == inputModel.Id);

            if (item != null)
            {
                item.ExerciseNumber = inputModel.ExerciseNumber;
                item.Title = inputModel.Title;
                item.Location = inputModel.Location;
                item.Rules = inputModel.Rules;
                item.Props = inputModel.Props;
                item.ExerciseType = inputModel.ExerciseType;
                item.OrderType = inputModel.OrderType;
                item.NumberOfPlayers = inputModel.NumberOfPlayers;

                await db.SaveChangesAsync();

                await CloseModalOk();
            }
            else await CloseModalErr();
        }
        catch
        {
            await CloseModalErr();
        }
    }

    protected async Task DeleteItem()
    {
        try
        {
            var gameResults = db.GameResult.Where(x => x.ExerciseId == inputModel.Id).ToList();

            if (gameResults != null && gameResults.Count == 0)
            {
                db.Exercise.Remove(ExerciseModel);
                await db.SaveChangesAsync();
                await CloseModalOk();
            }
            else toast.ShowWarning("Naloge ni mogoče izbrisati, saj ima povezavo v drugi tabeli!");
        }
        catch
        {
            await CloseModalErr();
        }
    }

    protected class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Številka naloge je obvezna!")]
        public int ExerciseNumber { get; set; }

        [Required(ErrorMessage = "Naslov naloge je obvezen!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Lokacija naloge je obvezna!")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Pravila so obvezna!")]
        public string Rules { get; set; } = "";

        [Required(ErrorMessage = "Rekviziti so obvezni!")]
        public string Props { get; set; }

        [Required(ErrorMessage = "Tip naloge je obvezen!")]
        public ExerciseType ExerciseType { get; set; }

        [Required(ErrorMessage = "Tip razvrščanja je obvezen!")]
        public OrderType OrderType { get; set; }

        [Required(ErrorMessage = "Število igralcev je obvezno!")]
        public int NumberOfPlayers { get; set; }
    }
}
