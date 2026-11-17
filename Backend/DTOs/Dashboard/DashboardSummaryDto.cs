
namespace PersonalBudgetManager.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalIncome { get; set; }

    public decimal TotalExpenses { get; set; }

    public decimal Balance { get; set; }

    public decimal CurrentMonthIncome { get; set; }

    public decimal CurrentMonthExpenses { get; set; }

    public BudgetOverviewDto? CurrentMonthBudget { get; set; }

    public List<MonthlySummaryDto> MonthlySummaries { get; set; } = new();

    public List<CategoryExpenseDto> CategoryExpenses { get; set; } = new();

    public List<RecentTransactionDto> RecentTransactions { get; set; } = new();
}
