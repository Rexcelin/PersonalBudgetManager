
using Microsoft.EntityFrameworkCore;
using PersonalBudgetManager.Data;
using PersonalBudgetManager.Models;
using PersonalBudgetManager.Repositories.Interfaces;

namespace PersonalBudgetManager.Repositories.Implementations;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(
        int transactionId,
        string userId)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t =>
                t.TransactionId == transactionId &&
                t.UserId == userId);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);

        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);

        await _context.SaveChangesAsync();
    }
}
