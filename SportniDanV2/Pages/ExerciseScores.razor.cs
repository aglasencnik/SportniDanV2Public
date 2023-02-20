using Microsoft.AspNetCore.Components;
using SportniDanV2.Data;
using SportniDanV2.Models;

namespace SportniDanV2.Pages;

public class ExerciseScoresBase : ComponentBase
{
    [Inject] ApplicationDbContext db { get; set; } = null!;

    [Inject] NavigationManager navManager { get; set; } = null!;

    protected List<GameResultModel> GameResults = new();
    protected List<ExerciseModel> Exercises = new();
    protected List<ClassModel> Classes = new();

    protected override void OnInitialized()
    {
        Exercises = db.Exercise.ToList();

        if (Exercises != null && Exercises.Count != 0)
        {
            Classes = db.Class.ToList();
            GameResults = db.GameResult.ToList();
        }
        else navManager.NavigateTo("/");
    }
}
