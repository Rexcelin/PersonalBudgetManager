
using PersonalBudgetManager.DTOs.Budgets;

namespace PersonalBudgetManager.Services.Interfaces;

public interface IBudgetService
{
    Task<List<BudgetResponseDto>> GetAllAsync(string userId);

    Task<BudgetResponseDto?> GetByIdAsync(
        int budgetId,
        string userId);

    Task<BudgetResponseDto> CreateAsync(
        CreateBudgetRequestDto request,
        string userId);

    Task<BudgetResponseDto?> UpdateAsync(
        int budgetId,
        UpdateBudgetRequestDto request,
        string userId);

    Task<bool> DeleteAsync(
        int budgetId,
        string userId);
}
