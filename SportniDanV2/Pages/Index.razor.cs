using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportniDanV2.Data;
using SportniDanV2.Enums;
using SportniDanV2.Models;
using SportniDanV2.Services;

namespace SportniDanV2.Pages;

public class IndexBase : ComponentBase
{
    [Inject] ApplicationDbContext db { get; set; } = null!;

    protected AppDataModel appData = new();

    protected List<GameResultModel> GameResults = new();
    protected List<ExerciseModel> Exercises = new();
    protected List<ClassModel> Classes = new();
    protected int numOfExercises = 0;
    protected List<GameScoresModel> gameScores = new();

    protected override void OnInitialized()
    {
        appData = db.AppData.FirstOrDefault(x => x.DataType == DataType.HomePageHtml);

        GameResults = db.GameResult.ToList();
        Exercises = db.Exercise.ToList();
        Classes = db.Class.ToList();

        numOfExercises = Exercises.Count();

        gameScores = GameScoringService.ScoreGame(Classes, Exercises, GameResults);
    }
}
