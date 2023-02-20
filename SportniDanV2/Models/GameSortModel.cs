namespace SportniDanV2.Models;

public class GameSortModel
{
    public int? Place { get; set; }
    public int? ExerciseId { get; set; }
    public ClassModel Class { get; set; }
    public ExerciseModel? Exercise { get; set; }
    public GameResultModel GameResult { get; set; }
}
