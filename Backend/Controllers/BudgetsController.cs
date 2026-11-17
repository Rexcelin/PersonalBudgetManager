
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetManager.DTOs.Budgets;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // GET: api/budgets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var budgets = await _budgetService.GetAllAsync(userId);

        return Ok(budgets);
    }

    // GET: api/budgets/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var budget = await _budgetService.GetByIdAsync(id, userId);

        if (budget == null)
        {
            return NotFound(new
            {
                message = "Budget not found."
            });
        }

        return Ok(budget);
    }

    // POST: api/budgets
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBudgetRequestDto request)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var budget = await _budgetService.CreateAsync(
                request,
                userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = budget.BudgetId },
                budget);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/budgets/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateBudgetRequestDto request)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var budget = await _budgetService.UpdateAsync(
            id,
            request,
            userId);

        if (budget == null)
        {
            return NotFound(new
            {
                message = "Budget not found."
            });
        }

        return Ok(budget);
    }

    // DELETE: api/budgets/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var deleted = await _budgetService.DeleteAsync(id, userId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Budget not found."
            });
        }

        return Ok(new
        {
            message = "Budget deleted successfully."
        });
    }
}
