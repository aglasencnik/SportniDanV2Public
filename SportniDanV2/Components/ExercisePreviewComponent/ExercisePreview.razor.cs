using Microsoft.AspNetCore.Components;
using SportniDanV2.Enums;
using SportniDanV2.Models;

namespace SportniDanV2.Components.ExercisePreviewComponent;

public class ExercisePreviewBase : ComponentBase
{
    [Parameter] public ExercisePreviewType ExercisePreviewType { get; set; }

    [Parameter] public ExerciseModel Exercise { get; set; }
}
