
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalBudgetManager.Models;

public class Budget
{
    [Key]
    public int BudgetId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal MonthlyLimit { get; set; }

    [Required]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100)]
    public int Year { get; set; }
}
