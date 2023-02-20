using SportniDanV2.Enums;
using SportniDanV2.Models;

namespace SportniDanV2.Services;

public class GameSortService
{
    public static List<GameSortModel> SortGames(List<GameSortModel> models, OrderType orderType)
    {
        int place = 1;

        if (orderType == OrderType.Ascending)
        {
            models = models.OrderBy(x => x.GameResult?.Result ?? Double.PositiveInfinity).ToList();

            var previousModel = new GameSortModel();

            for (int i = 0; i < models.Count(); i++)
            {
                if (previousModel.Place != null)
                {
                    if (models[i].GameResult != null)
                    {
                        if (models[i].GameResult.Result > previousModel.GameResult.Result)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                    else
                    {
                        if (previousModel.GameResult != null)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                }
                else
                {
                    models[i].Place = place;
                    previousModel = models[i];
                    place++;
                }
            }
        }
        else
        {
            models = models.OrderByDescending(x => x.GameResult?.Result ?? Double.NegativeInfinity).ToList();

            var previousModel = new GameSortModel();

            for (int i = 0; i < models.Count(); i++)
            {
                if (previousModel.Place != null)
                {
                    if (models[i].GameResult != null)
                    {
                        if (models[i].GameResult.Result < previousModel.GameResult.Result)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                    else
                    {
                        if (previousModel.GameResult != null)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                }
                else
                {
                    models[i].Place = place;
                    previousModel = models[i];
                    place++;
                }
            }
        }

        return models;
    }

    public static List<GameScoresModel> SortGameScores(List<GameScoresModel> models)
    {
        int place = 1;

        models = models.OrderBy(x => x.Points).ToList();

        var previousModel = new GameScoresModel();

        for (int i = 0; i < models.Count(); i++)
        {
            if (previousModel.Place != 0)
            {
                if (models[i].Points > previousModel.Points)
                {
                    models[i].Place = place;
                    previousModel = models[i];
                    place++;
                }
                else
                {
                    models[i].Place = previousModel.Place;
                    place++;
                }
            }
            else
            {
                models[i].Place = place;
                previousModel = models[i];
                place++;
            }
        }

        return models;
    }
}
