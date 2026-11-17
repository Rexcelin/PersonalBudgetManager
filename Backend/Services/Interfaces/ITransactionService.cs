
using PersonalBudgetManager.DTOs.Transactions;

namespace PersonalBudgetManager.Services.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetAllAsync(string userId);

    Task<TransactionResponseDto?> GetByIdAsync(
        int transactionId,
        string userId);

    Task<TransactionResponseDto> CreateAsync(
        CreateTransactionRequestDto request,
        string userId);

    Task<TransactionResponseDto?> UpdateAsync(
        int transactionId,
        UpdateTransactionRequestDto request,
        string userId);

    Task<bool> DeleteAsync(int transactionId, string userId);
}
