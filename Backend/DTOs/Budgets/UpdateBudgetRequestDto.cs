
using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetManager.DTOs.Budgets;

public class UpdateBudgetRequestDto
{
    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal MonthlyLimit { get; set; }
}
