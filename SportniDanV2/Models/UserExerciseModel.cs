using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportniDanV2.Models;

[Table("UserExercise")]
public class UserExerciseModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ExerciseId { get; set; }

    public UserModel User { get; set; } = null!;
    public ExerciseModel Exercise { get; set; } = null!;
}
