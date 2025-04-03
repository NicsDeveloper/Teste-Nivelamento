using MediatR;
using Questao5.Domain.Errors;

namespace Questao5.Application.Balance.Queries;

public class GetBalanceByIdQuery : IRequest<Result<GetBalanceByIdResponse>>
{
    public string CurrentAccountId { get; set; } = string.Empty;
}