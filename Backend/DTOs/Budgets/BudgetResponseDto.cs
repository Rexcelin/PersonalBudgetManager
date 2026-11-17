
namespace PersonalBudgetManager.DTOs.Budgets;

public class BudgetResponseDto
{
    public int BudgetId { get; set; }

    public decimal MonthlyLimit { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }
}
