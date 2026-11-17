
using PersonalBudgetManager.DTOs.Budgets;
using PersonalBudgetManager.Models;
using PersonalBudgetManager.Repositories.Interfaces;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Services.Implementations;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;

    public BudgetService(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<List<BudgetResponseDto>> GetAllAsync(
        string userId)
    {
        var budgets =
            await _budgetRepository.GetAllByUserIdAsync(userId);

        return budgets.Select(MapToResponse).ToList();
    }

    public async Task<BudgetResponseDto?> GetByIdAsync(
        int budgetId,
        string userId)
    {
        var budget =
            await _budgetRepository.GetByIdAsync(budgetId, userId);

        if (budget == null)
        {
            return null;
        }

        return MapToResponse(budget);
    }

    public async Task<BudgetResponseDto> CreateAsync(
        CreateBudgetRequestDto request,
        string userId)
    {
        bool alreadyExists =
            await _budgetRepository.ExistsForMonthAsync(
                userId,
                request.Month,
                request.Year);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "A budget already exists for this month and year.");
        }

        var budget = new Budget
        {
            UserId = userId,
            MonthlyLimit = request.MonthlyLimit,
            Month = request.Month,
            Year = request.Year
        };

        var createdBudget =
            await _budgetRepository.AddAsync(budget);

        return MapToResponse(createdBudget);
    }

    public async Task<BudgetResponseDto?> UpdateAsync(
        int budgetId,
        UpdateBudgetRequestDto request,
        string userId)
    {
        var budget =
            await _budgetRepository.GetByIdAsync(budgetId, userId);

        if (budget == null)
        {
            return null;
        }

        budget.MonthlyLimit = request.MonthlyLimit;

        await _budgetRepository.UpdateAsync(budget);

        return MapToResponse(budget);
    }

    public async Task<bool> DeleteAsync(
        int budgetId,
        string userId)
    {
        var budget =
            await _budgetRepository.GetByIdAsync(budgetId, userId);

        if (budget == null)
        {
            return false;
        }

        await _budgetRepository.DeleteAsync(budget);

        return true;
    }

    private static BudgetResponseDto MapToResponse(Budget budget)
    {
        return new BudgetResponseDto
        {
            BudgetId = budget.BudgetId,
            MonthlyLimit = budget.MonthlyLimit,
            Month = budget.Month,
            Year = budget.Year
        };
    }
}
