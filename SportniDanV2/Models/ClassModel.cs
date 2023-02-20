using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportniDanV2.Models;

[Table("Class")]
public class ClassModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<GameResultModel> GameResults { get; set; } = null!;
}
