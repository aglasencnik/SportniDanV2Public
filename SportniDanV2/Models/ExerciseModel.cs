using SportniDanV2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportniDanV2.Models;

[Table("Exercise")]
public class ExerciseModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ExerciseNumber { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Location { get; set; } = null!;

    [Required]
    public string Rules { get; set; } = null!;

    [Required]
    public string Props { get; set; } = null!;

    [Required]
    public ExerciseType ExerciseType { get; set; }

    [Required]
    public OrderType OrderType { get; set; }

    [Required]
    public int NumberOfPlayers { get; set; }

    public ICollection<UserExerciseModel> UserExercises { get; set; } = null!;
}
