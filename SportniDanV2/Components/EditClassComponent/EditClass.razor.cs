using Blazored.Modal.Services;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using SportniDanV2.Models;
using System.ComponentModel.DataAnnotations;
using Blazored.Toast.Services;
using SportniDanV2.Data;

namespace SportniDanV2.Components.EditClassComponent;

public class EditClassBase : ComponentBase
{
    [Inject] IToastService toast { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter] public Enums.EditType EditType { get; set; }

    [Parameter] public ClassModel? ClassModel { get; set; } = new();

    protected InputModel inputModel = new();

    protected string errorMessage = "";

    protected override void OnInitialized()
    {
        if (ClassModel != null)
        {
            inputModel.Id = ClassModel.Id;
            inputModel.Name = ClassModel.Name;
        }
    }

    protected async Task FormSubmit()
    {
        if (inputModel.Name != "")
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
            var model = new ClassModel() { Name = inputModel.Name };
            await db.Class.AddAsync(model);
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
            var item = db.Class.FirstOrDefault(x => x.Id == inputModel.Id);

            if (item != null)
            {
                item.Name = inputModel.Name;
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
            var gameResults = db.GameResult.Where(x => x.ClassId == inputModel.Id).ToList();

            if (gameResults != null && gameResults.Count == 0)
            {
                db.Class.Remove(ClassModel);
                await db.SaveChangesAsync();
                await CloseModalOk();
            }
            else toast.ShowWarning("Razreda ni mogoče izbrisati, saj ima povezavo v drugi tabeli!");
        }
        catch
        {
            await CloseModalErr();
        }
    }

    protected class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime razreda je obvezno!")]
        public string Name { get; set; } = null!;
    }
}
