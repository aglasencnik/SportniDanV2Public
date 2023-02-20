using SportniDanV2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataType = SportniDanV2.Enums.DataType;

namespace SportniDanV2.Models;

[Table("AppData")]
public class AppDataModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DataType DataType { get; set; }

    [Required]
    public string Value { get; set; } = "";
}
