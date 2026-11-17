
using PersonalBudgetManager.DTOs.Transactions;
using PersonalBudgetManager.Enums;
using PersonalBudgetManager.Models;
using PersonalBudgetManager.Repositories.Interfaces;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(
        ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<List<TransactionResponseDto>> GetAllAsync(
        string userId)
    {
        var transactions =
            await _transactionRepository.GetAllByUserIdAsync(userId);

        return transactions.Select(MapToResponse).ToList();
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(
        int transactionId,
        string userId)
    {
        var transaction =
            await _transactionRepository.GetByIdAsync(
                transactionId, userId);

        if (transaction == null)
        {
            return null;
        }

        return MapToResponse(transaction);
    }

    public async Task<TransactionResponseDto> CreateAsync(
        CreateTransactionRequestDto request,
        string userId)
    {
        if (!Enum.TryParse<TransactionType>(
            request.Type, true, out var transactionType) ||
            !Enum.IsDefined(transactionType))
        {
            throw new ArgumentException(
                "Transaction type must be Income or Expense.");
        }

        if (request.TransactionDate == null ||
            request.TransactionDate.Value == default)
        {
            throw new ArgumentException(
                "A valid transaction date is required.");
        }

        var transaction = new Transaction
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Amount = request.Amount,
            Type = transactionType,
            Category = request.Category.Trim(),
            TransactionDate = request.TransactionDate.Value,
            Description = request.Description?.Trim()
        };

        var createdTransaction =
            await _transactionRepository.AddAsync(transaction);

        return MapToResponse(createdTransaction);
    }

    public async Task<TransactionResponseDto?> UpdateAsync(
        int transactionId,
        UpdateTransactionRequestDto request,
        string userId)
    {
        var transaction =
            await _transactionRepository.GetByIdAsync(
                transactionId, userId);

        if (transaction == null)
        {
            return null;
        }

        if (!Enum.TryParse<TransactionType>(
            request.Type, true, out var transactionType) ||
            !Enum.IsDefined(transactionType))
        {
            throw new ArgumentException(
                "Transaction type must be Income or Expense.");
        }

        if (request.TransactionDate == null ||
            request.TransactionDate.Value == default)
        {
            throw new ArgumentException(
                "A valid transaction date is required.");
        }

        transaction.Title = request.Title.Trim();
        transaction.Amount = request.Amount;
        transaction.Type = transactionType;
        transaction.Category = request.Category.Trim();
        transaction.TransactionDate = request.TransactionDate.Value;
        transaction.Description = request.Description?.Trim();

        await _transactionRepository.UpdateAsync(transaction);

        return MapToResponse(transaction);
    }

    public async Task<bool> DeleteAsync(
        int transactionId,
        string userId)
    {
        var transaction =
            await _transactionRepository.GetByIdAsync(
                transactionId, userId);

        if (transaction == null)
        {
            return false;
        }

        await _transactionRepository.DeleteAsync(transaction);

        return true;
    }

    private static TransactionResponseDto MapToResponse(
        Transaction transaction)
    {
        return new TransactionResponseDto
        {
            TransactionId = transaction.TransactionId,
            Title = transaction.Title,
            Amount = transaction.Amount,
            Type = transaction.Type.ToString(),
            Category = transaction.Category,
            TransactionDate = transaction.TransactionDate,
            Description = transaction.Description
        };
    }
}
