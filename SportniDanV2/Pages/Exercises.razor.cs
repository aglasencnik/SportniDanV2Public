using Microsoft.AspNetCore.Components;
using SportniDanV2.Data;
using SportniDanV2.Models;

namespace SportniDanV2.Pages;

public class ExercisesBase : ComponentBase
{
    [Inject] ApplicationDbContext db { get; set; } = null!;

    protected List<ExerciseModel> exercises = new();

    protected override void OnInitialized()
    {
        exercises = db.Exercise.ToList();
    }
}
