
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetManager.DTOs.Transactions;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(
        ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException(
                "User ID was not found in the token.");
    }

    // GET: api/transactions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        var transactions =
            await _transactionService.GetAllAsync(userId);

        return Ok(transactions);
    }

    // GET: api/transactions/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();

        var transaction =
            await _transactionService.GetByIdAsync(id, userId);

        if (transaction == null)
        {
            return NotFound(new
            {
                message = "Transaction not found."
            });
        }

        return Ok(transaction);
    }

    // POST: api/transactions
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTransactionRequestDto request)
    {
        var userId = GetUserId();

        try
        {
            var transaction =
                await _transactionService.CreateAsync(request, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = transaction.TransactionId },
                transaction);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/transactions/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTransactionRequestDto request)
    {
        var userId = GetUserId();

        try
        {
            var transaction =
                await _transactionService.UpdateAsync(
                    id, request, userId);

            if (transaction == null)
            {
                return NotFound(new
                {
                    message = "Transaction not found."
                });
            }

            return Ok(transaction);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/transactions/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        var deleted =
            await _transactionService.DeleteAsync(id, userId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Transaction not found."
            });
        }

        return Ok(new
        {
            message = "Transaction deleted successfully."
        });
    }
}
