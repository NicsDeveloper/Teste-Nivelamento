using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Balance.Queries;

namespace Questao5.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BalanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public BalanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get an account balance.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromBody] GetBalanceByIdQuery request)
    {
        var result = await _mediator.Send(request);

        if (result.Success)
            return Ok(result.Data);
        else
            return BadRequest(new
            {
                Type = result.ErrorType,
                Message = result.ErrorMessage
            });
    }
}

