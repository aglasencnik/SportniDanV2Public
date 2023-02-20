using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SportniDanV2.Pages.InsertResults;

public class InsertResultsBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; }

    [Inject] ApplicationDbContext db { get; set; }

    [Inject] IToastService toast { get; set; }

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    [Parameter] public int ExerciseId { get; set; }

    protected List<ClassModel> Classes = new();
    protected ExerciseModel Exercise = new();
    protected InputModel inputModel = new();
    protected string errorMessage = "";
    protected List<double> PlayerInputsList = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0) NavManager.NavigateTo("/");

        if (user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            if (!CheckIfAuthorized(user)) NavManager.NavigateTo("/");

        Exercise = db.Exercise.FirstOrDefault(x => x.Id == ExerciseId);

        if (Exercise != null)
        {
            Classes = db.Class.ToList();

            ResetInputs();
        }
        else NavManager.NavigateTo("/");
    }

    protected async Task FormSubmit()
    {
        try
        {
            if (inputModel != null && inputModel.ClassId != 0)
            {
                var existingResult = db.GameResult.FirstOrDefault(x => x.ClassId == inputModel.ClassId && x.ExerciseId == Exercise.Id);

                string playerResults = "";

                foreach (var item in PlayerInputsList)
                {
                    playerResults += $"{item.ToString()};";
                }

                if (existingResult != null)
                {
                    existingResult.Result = inputModel.Result;
                    existingResult.PlayerResults = playerResults;
                }
                else
                {
                    await db.GameResult.AddAsync(new GameResultModel()
                    {
                        ExerciseId = Exercise.Id,
                        ClassId = inputModel.ClassId,
                        Result = inputModel.Result,
                        PlayerResults = playerResults
                    });
                }

                await db.SaveChangesAsync();

                ResetInputs();

                toast.ShowSuccess("Vpis naloge uspešen!");
            }
            else errorMessage = "Nepopoln vpis!";
        }
        catch
        {
            toast.ShowError("Napaka pri vpisu!");
        }
    }

    protected void resum(int i, Microsoft.AspNetCore.Components.ChangeEventArgs e)
    {
        double n = 0;
        if (Double.TryParse(e.Value.ToString(), out double auxn)) n = auxn;
        PlayerInputsList[i] = n;
        inputModel.Result = PlayerInputsList.Sum(x => x);
    }

    protected bool CheckIfAuthorized(ClaimsPrincipal user)
    {
        var username = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var password = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Hash)?.Value;

        var userId = db.User.FirstOrDefault(x => x.Username == username && x.Password == password).Id;

        if (userId != 0)
        {
            var userExercises = db.UserExercise.FirstOrDefault(x => x.UserId == userId);

            if (userExercises == null) return false;
            else return true;
        }
        else return false;
    }

    protected void ResetInputs()
    {
        PlayerInputsList.Clear();
        int N = Exercise.NumberOfPlayers;

        if (N > 1)
        {
            PlayerInputsList = new List<double>(new double[N]);
        }
        inputModel = new InputModel();
        errorMessage = "";
    }

    protected class InputModel
    {
        [Required(ErrorMessage = "Razred je obvezen!")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Rezultat je obvezen!")]
        public double Result { get; set; }
    }
}
