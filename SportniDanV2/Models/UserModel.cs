using SportniDanV2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportniDanV2.Models;

[Table("User")]
public class UserModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public UserType UserType { get; set; }

    public ICollection<UserExerciseModel> UserExercises { get; set; } = null!;
}
