using MediatR;
using Questao5.Application.Interfaces;
using Questao5.Domain.Errors;

namespace Questao5.Application.Balance.Queries.Handlers;

public class GetBalanceByIdHandler : IRequestHandler<GetBalanceByIdQuery, Result<GetBalanceByIdResponse>>
{
    private readonly ICurrentAccountRepository _accountRepository;
    private readonly IMovementRepository _movementRepository;

    public GetBalanceByIdHandler(
        ICurrentAccountRepository accountRepository,
        IMovementRepository movementRepository
    )
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _movementRepository = movementRepository ?? throw new ArgumentNullException(nameof(movementRepository));
    }

    public async Task<Result<GetBalanceByIdResponse>> Handle(GetBalanceByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetCurrentAccountByIdAsync(request.CurrentAccountId)
            ?? throw new ArgumentException("INVALID_ACCOUNT: Current account not found.");

        if (!account.Active)
            throw new ArgumentException("INACTIVE_ACCOUNT: Current account is inactive.");

        var balance = await _movementRepository.GetBalance(request.CurrentAccountId);

        var result = new GetBalanceByIdResponse
        {
            AccountNumber = account.Number,
            AccountName = account.Name,
            Balance = balance,
            ResponseDateTime = DateTime.UtcNow.ToString("o")
        };

        return Result<GetBalanceByIdResponse>.Ok(result);
    }
}