
using Microsoft.EntityFrameworkCore;
using PersonalBudgetManager.Data;
using PersonalBudgetManager.Models;
using PersonalBudgetManager.Repositories.Interfaces;

namespace PersonalBudgetManager.Repositories.Implementations;

public class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Budget>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Budgets
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ToListAsync();
    }

    public async Task<Budget?> GetByIdAsync(
        int budgetId,
        string userId)
    {
        return await _context.Budgets
            .FirstOrDefaultAsync(b =>
                b.BudgetId == budgetId &&
                b.UserId == userId);
    }

    public async Task<bool> ExistsForMonthAsync(
        string userId,
        int month,
        int year,
        int? excludeBudgetId = null)
    {
        return await _context.Budgets.AnyAsync(b =>
            b.UserId == userId &&
            b.Month == month &&
            b.Year == year &&
            (!excludeBudgetId.HasValue ||
             b.BudgetId != excludeBudgetId.Value));
    }

    public async Task<Budget> AddAsync(Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();

        return budget;
    }

    public async Task UpdateAsync(Budget budget)
    {
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Budget budget)
    {
        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
    }
}
