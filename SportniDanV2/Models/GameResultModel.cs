using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportniDanV2.Models;

[Table("GameResult")]
public class GameResultModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public double Result { get; set; }

    public string PlayerResults { get; set; } = null!;

    public ClassModel Class { get; set; } = null!;
    public ExerciseModel Exercise { get; set; } = null!;
}
