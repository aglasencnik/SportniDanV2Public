using SportniDanV2.Models;
using System.Collections.Generic;

namespace SportniDanV2.Services;

public class GameScoringService
{
    public static List<GameScoresModel> ScoreGame(List<ClassModel> classes, List<ExerciseModel> exercises, List<GameResultModel> gameResults)
    {
        List<GameScoresModel> gameScores = InitializeGameScores(classes, classes.Count, exercises.Count);

        foreach (var exercise in exercises)
        {
            List<GameSortModel> sortedGames = new List<GameSortModel>();

            foreach (var item in classes)
            {
                var gameResult = gameResults.FirstOrDefault(x => x.ClassId == item.Id && x.ExerciseId == exercise.Id);

                sortedGames.Add(new GameSortModel()
                {
                    ExerciseId = exercise.Id,
                    Class = item,
                    Exercise = exercise,
                    GameResult = gameResult
                });
            }

            sortedGames = GameSortService.SortGames(sortedGames, exercise.OrderType);

            foreach (var sortedGame in sortedGames)
            {
                int minusScore = 0;

                if (sortedGame.GameResult != null) minusScore = classes.Count - (int)sortedGame.Place;

                int index = gameScores.FindIndex(x => x.ClassId == sortedGame.Class.Id);
                gameScores[index].Points -= minusScore;
                gameScores[index].ExercisePlaces.Add(sortedGame.GameResult != null ? (int)sortedGame.Place : 0);
            }
        }

        gameScores = GameSortService.SortGameScores(gameScores);

        return gameScores;
    }

    private static List<GameScoresModel> InitializeGameScores(List<ClassModel> classes, int numOfClasses, int numOfExercises)
    {
        List<GameScoresModel> gameScores = new List<GameScoresModel>();

        foreach (var item in classes)
        {
            gameScores.Add(new GameScoresModel()
            {
                ClassName = item.Name,
                ClassId = item.Id,
                Points = numOfClasses * numOfExercises,
                Place = 0,
                ExercisePlaces = new List<int>()
            });
        }

        return gameScores;
    }
}
