using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Data.Entities;
using PaymentsService.Dtos; 

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(PaymentsDbContext context, ILogger<AccountsController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId не может быть пустым.");
        }

        var accountExists = await context.Accounts.AnyAsync(a => a.UserId == userId);
        if (accountExists)
        {
            return Conflict($"Счет для пользователя {userId} уже существует.");
        }

        var account = new Account
        {
            UserId = userId,
            Balance = 0
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Создан счет для пользователя {UserId}", userId);
        
        var accountDto = new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance
        };

        return CreatedAtAction(nameof(GetBalance), new { userId = userId }, accountDto);
    }
    
    [HttpGet("{userId}/balance")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetBalance(string userId)
    {
        var account = await context.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (account == null)
        {
            return NotFound($"Счет для пользователя {userId} не найден.");
        }

        var accountDto = new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance
        };

        return Ok(accountDto);
    }
    
    [HttpPost("{userId}/deposit")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> Deposit(string userId, [FromQuery] decimal amount)
    {
        if (amount <= 0)
        {
            return BadRequest("Сумма пополнения должна быть положительной.");
        }
        
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
        {
            return NotFound($"Счет для пользователя {userId} не найден.");
        }

        account.Balance += amount;
        await context.SaveChangesAsync();
        
        logger.LogInformation("Счет пользователя {UserId} пополнен на {Amount}. Новый баланс: {Balance}", userId, amount, account.Balance);
        
        var accountDto = new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance
        };
        
        return Ok(accountDto);
    }
}