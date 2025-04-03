using MediatR;
using Questao5.Domain.Errors;

namespace Questao5.Application.Movement;
public class CreateMovementCommand : IRequest<Result<CreateMovementResponse>>
{
    public string CurrentAccountId { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string Idempotencia { get; set; } = string.Empty;
}
