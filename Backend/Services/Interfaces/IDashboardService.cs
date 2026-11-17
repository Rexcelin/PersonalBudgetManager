
using PersonalBudgetManager.DTOs.Dashboard;

namespace PersonalBudgetManager.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId);
}
