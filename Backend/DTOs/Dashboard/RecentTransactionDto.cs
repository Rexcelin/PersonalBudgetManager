
namespace PersonalBudgetManager.DTOs.Dashboard;

public class RecentTransactionDto
{
    public int TransactionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; }

    public string? Description { get; set; }
}
