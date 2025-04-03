using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Interfaces;
using Questao5.Domain.Errors;

namespace Questao5.Application.Movement;
public class CreateMovementHandler : IRequestHandler<CreateMovementCommand, Result<CreateMovementResponse>>
{
    private readonly ICurrentAccountRepository _accountRepository;
    private readonly IMovementRepository _movementRepository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;

    public CreateMovementHandler(
        ICurrentAccountRepository accountRepository,
        IMovementRepository movementRepository,
        IIdempotenciaRepository idempotenciaRepository)
    {
        _accountRepository = accountRepository;
        _movementRepository = movementRepository;
        _idempotenciaRepository = idempotenciaRepository;
    }

    public async Task<Result<CreateMovementResponse>> Handle(CreateMovementCommand request, CancellationToken cancellationToken)
    {

        if (request.Value <= 0)
            throw new ArgumentException("INVALID_VALUE: Value must be positive.");

        if (request.MovementType != "C" && request.MovementType != "D")
            throw new ArgumentException("INVALID_TYPE: Invalid type. Must be 'C' or 'D'.");

        var account = await _accountRepository.GetCurrentAccountByIdAsync(request.CurrentAccountId) ?? throw new ArgumentException("INVALID_ACCOUNT: Account not found.");

        if (!account.Active)
            throw new ArgumentException("INACTIVE_ACCOUNT: Account is inactive.");

        if (await _idempotenciaRepository.ExistsAsync(request.Idempotencia))
        {
            var resultadoExistenteObj = await _idempotenciaRepository.GetResultadoAsync(request.Idempotencia);

            var resultadoExistenteJson = resultadoExistenteObj.ToString()!;
            var resultadoExistente = JsonConvert.DeserializeObject<CreateMovementResponse>(resultadoExistenteJson)!;
            return Result<CreateMovementResponse>.Ok(new CreateMovementResponse { MovementId = resultadoExistente.MovementId });
        }

        var Movement = new Domain.Entities.Movement
        {
            MovementId = Guid.NewGuid().ToString(),
            CurrentAccountId = request.CurrentAccountId,
            MovementData = DateTime.UtcNow.ToString("dd/MM/yyyy"),
            MovementType = request.MovementType,
            Value = request.Value
        };

        await _movementRepository.InsertMovementAsync(Movement);

        var resultado = new CreateMovementResponse { MovementId = Movement.MovementId };
        await _idempotenciaRepository.SaveAsync(request.Idempotencia, resultado);

        return Result<CreateMovementResponse>.Ok(resultado);
    }
}
