using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Movement;

namespace Questao5.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovementController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create an account movement (credit or debit).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMovementCommand command)
    {
        var result = await _mediator.Send(command);

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

