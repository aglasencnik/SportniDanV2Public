namespace SportniDanV2.Models;

public class GameScoresModel
{
    public int Place { get; set; }
    public int Points { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public List<int> ExercisePlaces { get; set; }
}
