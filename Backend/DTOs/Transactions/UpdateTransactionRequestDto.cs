
using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetManager.DTOs.Transactions;

public class UpdateTransactionRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression("^(Income|Expense)$",
        ErrorMessage = "Type must be either Income or Expense.")]
    public string Type { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime? TransactionDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
