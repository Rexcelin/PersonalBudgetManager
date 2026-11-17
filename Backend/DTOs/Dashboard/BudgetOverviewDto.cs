
namespace PersonalBudgetManager.DTOs.Dashboard;

public class BudgetOverviewDto
{
    public decimal MonthlyLimit { get; set; }

    public decimal AmountSpent { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal UsagePercentage { get; set; }

    public bool IsOverBudget { get; set; }
}
