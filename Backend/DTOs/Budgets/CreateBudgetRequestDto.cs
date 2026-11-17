
using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetManager.DTOs.Budgets;

public class CreateBudgetRequestDto
{
    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal MonthlyLimit { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100.")]
    public int Year { get; set; }
}
