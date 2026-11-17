
using PersonalBudgetManager.DTOs.Dashboard;
using PersonalBudgetManager.Enums;
using PersonalBudgetManager.Repositories.Interfaces;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(
        string userId)
    {
        var today = DateTime.Today;

        var currentMonthStart = new DateTime(
            today.Year,
            today.Month,
            1);

        var nextMonthStart = currentMonthStart.AddMonths(1);

        // Retrieve all-time income and expenses.
        var totalIncome =
            await _dashboardRepository.GetTotalIncomeAsync(userId);

        var totalExpenses =
            await _dashboardRepository.GetTotalExpensesAsync(userId);

        // Retrieve transactions for the last six months.
        var chartStartDate = currentMonthStart.AddMonths(-5);

        var periodTransactions =
            await _dashboardRepository.GetTransactionsBetweenAsync(
                userId,
                chartStartDate,
                nextMonthStart);

        // Calculate current month income and expenses.
        var currentMonthTransactions = periodTransactions
            .Where(t =>
                t.TransactionDate >= currentMonthStart &&
                t.TransactionDate < nextMonthStart)
            .ToList();

        var currentMonthIncome = currentMonthTransactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var currentMonthExpenses = currentMonthTransactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        // Prepare six-month income and expense comparison.
        var monthlySummaries = new List<MonthlySummaryDto>();

        for (int i = 0; i < 6; i++)
        {
            var monthDate = chartStartDate.AddMonths(i);

            var monthTransactions = periodTransactions
                .Where(t =>
                    t.TransactionDate.Year == monthDate.Year &&
                    t.TransactionDate.Month == monthDate.Month)
                .ToList();

            monthlySummaries.Add(new MonthlySummaryDto
            {
                Month = monthDate.Month,
                Year = monthDate.Year,
                MonthName = monthDate.ToString("MMMM"),

                Income = monthTransactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount),

                Expenses = monthTransactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount)
            });
        }

        // Prepare current-month category-wise expenses.
        var categoryExpenses = currentMonthTransactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category)
                ? "Uncategorized"
                : t.Category)
            .Select(group => new CategoryExpenseDto
            {
                Category = group.Key,
                Amount = group.Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        // Retrieve the latest five transactions.
        var recentTransactions =
            await _dashboardRepository.GetRecentTransactionsAsync(
                userId,
                5);

        var recentTransactionDtos = recentTransactions
            .Select(t => new RecentTransactionDto
            {
                TransactionId = t.TransactionId,
                Title = t.Title,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Category = t.Category,
                TransactionDate = t.TransactionDate,
                Description = t.Description
            })
            .ToList();

        // Retrieve the current month's budget.
        var currentBudget =
            await _dashboardRepository.GetBudgetForMonthAsync(
                userId,
                today.Month,
                today.Year);

        BudgetOverviewDto? budgetOverview = null;

        if (currentBudget != null)
        {
            var remainingAmount =
                currentBudget.MonthlyLimit - currentMonthExpenses;

            var usagePercentage =
                currentBudget.MonthlyLimit > 0
                    ? Math.Round(
                        currentMonthExpenses /
                        currentBudget.MonthlyLimit * 100m,
                        2)
                    : 0m;

            budgetOverview = new BudgetOverviewDto
            {
                MonthlyLimit = currentBudget.MonthlyLimit,
                AmountSpent = currentMonthExpenses,
                RemainingAmount = remainingAmount,
                UsagePercentage = usagePercentage,
                IsOverBudget = currentMonthExpenses >
                               currentBudget.MonthlyLimit
            };
        }

        // Return the complete dashboard response.
        return new DashboardSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses,

            CurrentMonthIncome = currentMonthIncome,
            CurrentMonthExpenses = currentMonthExpenses,

            CurrentMonthBudget = budgetOverview,

            MonthlySummaries = monthlySummaries,
            CategoryExpenses = categoryExpenses,
            RecentTransactions = recentTransactionDtos
        };
    }
}
