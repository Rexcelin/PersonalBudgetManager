
using PersonalBudgetManager.Models;

namespace PersonalBudgetManager.Repositories.Interfaces;

public interface IBudgetRepository
{
    Task<List<Budget>> GetAllByUserIdAsync(string userId);

    Task<Budget?> GetByIdAsync(int budgetId, string userId);

    Task<bool> ExistsForMonthAsync(
        string userId,
        int month,
        int year,
        int? excludeBudgetId = null);

    Task<Budget> AddAsync(Budget budget);

    Task UpdateAsync(Budget budget);

    Task DeleteAsync(Budget budget);
}
