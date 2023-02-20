using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using System.ComponentModel.DataAnnotations;

namespace SportniDanV2.Components.EditUserComponent;

public class EditUserBase : ComponentBase
{
    [Inject] IToastService toast { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter] public Enums.EditType EditType { get; set; }

    [Parameter] public UserModel? UserModel { get; set; } = new();

    protected List<UserExerciseModel> UserExercises = new();

    protected List<ExerciseModel> Exercises = new();

    protected List<int> SelectedExerciseIds;

    protected List<int> PreSelectedIds = new();

    protected ElementReference _selectReference = new();

    protected InputModel inputModel = new();

    protected string errorMessage = "";

    protected override void OnInitialized()
    {
        if (UserModel != null)
        {
            inputModel.Id = UserModel.Id;
            inputModel.Username = UserModel.Username;
            inputModel.Password = UserModel.Password;
            inputModel.UserType = UserModel.UserType;

            if (UserModel.UserExercises != null)
            {
                inputModel.UserExercises = UserModel.UserExercises.ToList();
                foreach (var item in inputModel.UserExercises)
                    PreSelectedIds.Add(item.ExerciseId);
            }
        }

        UserExercises = db.UserExercise.ToList();
        Exercises = db.Exercise.ToList();
    }

    protected async Task FormSubmit()
    {
        if (inputModel.Username != "" && inputModel.Password != "")
        {
            if (EditType == Enums.EditType.Add) await CreateItem();
            else if (EditType == Enums.EditType.Edit) await EditItem();
            else await DeleteItem();
        }
        else
        {
            errorMessage = "Nezadostni vnosi podatkov!";
        }
    }

    protected async Task CloseModalOk() => await BlazoredModal.CloseAsync(ModalResult.Ok<string>("true"));

    protected async Task CloseModalErr() => await BlazoredModal.CloseAsync(ModalResult.Ok<string>("false"));

    protected async Task CreateItem()
    {
        try
        {
            var model = new UserModel()
            {
                Username = inputModel.Username,
                Password = inputModel.Password,
                UserType = inputModel.UserType
            };

            await db.User.AddAsync(model);
            await db.SaveChangesAsync();

            var user = db.User.FirstOrDefault(x => x.Username == inputModel.Username && x.Password == inputModel.Password);
            List<UserExerciseModel> exercisesToInput = new List<UserExerciseModel>();

            if (user != null)
            {
                if (SelectedExerciseIds != null)
                {
                    foreach (var exerciseId in SelectedExerciseIds)
                    {
                        exercisesToInput.Add(new UserExerciseModel() { ExerciseId = exerciseId, UserId = user.Id });
                    }
                }
            }

            if (exercisesToInput != null && exercisesToInput.Count() != 0)
            {
                await db.UserExercise.AddRangeAsync(exercisesToInput);
                await db.SaveChangesAsync();
            }

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
            var item = db.User.FirstOrDefault(x => x.Id == inputModel.Id);

            if (item != null)
            {
                item.Username = inputModel.Username;
                item.Password = inputModel.Password;
                item.UserType = inputModel.UserType;

                await db.SaveChangesAsync();

                List<UserExerciseModel> exercisesToInput = new List<UserExerciseModel>();
                if (SelectedExerciseIds != null)
                {
                    foreach (var exerciseId in SelectedExerciseIds)
                    {
                        exercisesToInput.Add(new UserExerciseModel() { ExerciseId = exerciseId, UserId = inputModel.Id });
                    }
                }

                var exercises = UserExercises.Where(x => x.UserId == inputModel.Id).ToList();

                if (exercises != null && exercises.Count() != 0) db.UserExercise.RemoveRange(exercises);

                if (exercisesToInput != null && exercisesToInput.Count() != 0)
                {
                    await db.UserExercise.AddRangeAsync(exercisesToInput);
                    await db.SaveChangesAsync();
                }

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
            if (UserModel.UserExercises != null) db.UserExercise.RemoveRange(UserModel.UserExercises);
            await db.SaveChangesAsync();
            db.User.Remove(UserModel);
            await db.SaveChangesAsync();
            await CloseModalOk();
        }
        catch
        {
            await CloseModalErr();
        }
    }

    protected void OnExerciseSelectionChanged(Microsoft.AspNetCore.Components.ChangeEventArgs e)
    {
        var tempExercises = (string[])e.Value;
        SelectedExerciseIds = new List<int>();
        foreach (var item in tempExercises)
        {
            SelectedExerciseIds.Add(Convert.ToInt32(item));
        }
    }

    protected class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Uporabniško ime je obvezno!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Tip uporabnika je obvezen!")]
        public UserType UserType { get; set; }

        public List<UserExerciseModel> UserExercises { get; set; }
    }
}
