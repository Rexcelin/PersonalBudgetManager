
using PersonalBudgetManager.Models;

namespace PersonalBudgetManager.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<decimal> GetTotalIncomeAsync(string userId);

    Task<decimal> GetTotalExpensesAsync(string userId);

    Task<List<Transaction>> GetTransactionsBetweenAsync(
        string userId,
        DateTime startDate,
        DateTime endDate);

    Task<List<Transaction>> GetRecentTransactionsAsync(
        string userId,
        int count);

    Task<Budget?> GetBudgetForMonthAsync(
        string userId,
        int month,
        int year);
}
