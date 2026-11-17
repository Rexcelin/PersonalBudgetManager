
using Microsoft.EntityFrameworkCore;
using PersonalBudgetManager.Data;
using PersonalBudgetManager.Enums;
using PersonalBudgetManager.Models;
using PersonalBudgetManager.Repositories.Interfaces;

namespace PersonalBudgetManager.Repositories.Implementations;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetTotalIncomeAsync(string userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId &&
                        t.Type == TransactionType.Income)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;
    }

    public async Task<decimal> GetTotalExpensesAsync(string userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId &&
                        t.Type == TransactionType.Expense)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;
    }

    public async Task<List<Transaction>> GetTransactionsBetweenAsync(
        string userId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _context.Transactions
            .Where(t =>
                t.UserId == userId &&
                t.TransactionDate >= startDate &&
                t.TransactionDate < endDate)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync(
        string userId,
        int count)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionId)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Budget?> GetBudgetForMonthAsync(
        string userId,
        int month,
        int year)
    {
        return await _context.Budgets
            .FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Month == month &&
                b.Year == year);
    }
}
