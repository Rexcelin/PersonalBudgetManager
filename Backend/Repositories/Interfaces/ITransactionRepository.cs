
using PersonalBudgetManager.Models;

namespace PersonalBudgetManager.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllByUserIdAsync(string userId);

    Task<Transaction?> GetByIdAsync(int transactionId, string userId);

    Task<Transaction> AddAsync(Transaction transaction);

    Task UpdateAsync(Transaction transaction);

    Task DeleteAsync(Transaction transaction);
}
