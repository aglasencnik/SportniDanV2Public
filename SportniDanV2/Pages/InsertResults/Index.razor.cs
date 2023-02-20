using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using System.Security.Claims;

namespace SportniDanV2.Pages.InsertResults;

public class InsertIndexBase : ComponentBase
{
    [Inject] NavigationManager NavManager { get; set; }

    [Inject] ApplicationDbContext db { get; set; }

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; }

    protected List<ExerciseModel> exercises = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0)
            NavManager.NavigateTo("/");

        if (user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value == UserType.Admin.ToString())
            exercises = db.Exercise.ToList();
        else
        {
            var username = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var password = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Hash)?.Value;

            var userId = db.User.FirstOrDefault(x => x.Username == username && x.Password == password).Id;

            if (userId != 0)
            {
                var userExercises = db.UserExercise.Where(x => x.UserId == userId).ToList();
                List<int> availableExerciseIds = new();

                foreach (var item in userExercises)
                {
                    availableExerciseIds.Add(item.ExerciseId);
                }

                if (availableExerciseIds != null && availableExerciseIds.Count() != 0)
                {
                    exercises = db.Exercise.Where(x => availableExerciseIds.Contains(x.Id)).ToList();

                    if (exercises.Count() == 1) NavManager.NavigateTo($"/insertresults/{exercises[0].Id}");
                }
            }
        }
    }
}
