using Microsoft.AspNetCore.Components;
using SportniDanV2.Data;
using SportniDanV2.Models;

namespace SportniDanV2.Pages;

public class ExerciseDetailsBase : ComponentBase
{
    [Inject] NavigationManager navManager { get; set; } = null!;

    [Inject] ApplicationDbContext db { get; set; } = null!;

    [Parameter] public int ExerciseId { get; set; }

    protected List<GameResultModel> GameResults = new();
    protected ExerciseModel Exercise = new();
    protected List<ClassModel> Classes = new();

    protected override void OnInitialized()
    {
        if (ExerciseId != 0)
        {
            Exercise = db.Exercise.FirstOrDefault(x => x.Id == ExerciseId);

            if (Exercise != null)
            {
                GameResults = db.GameResult.Where(x => x.ExerciseId == ExerciseId).ToList();
                Classes = db.Class.ToList();
            }
            else navManager.NavigateTo("/");
        }
        else navManager.NavigateTo("/");
    }
}
