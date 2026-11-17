
namespace PersonalBudgetManager.DTOs.Dashboard;

public class MonthlySummaryDto
{
    public int Month { get; set; }

    public int Year { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public decimal Income { get; set; }

    public decimal Expenses { get; set; }
}
