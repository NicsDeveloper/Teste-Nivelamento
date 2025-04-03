using Questao5.Domain.Entities;

namespace Questao5.Application.Interfaces
{
    public interface IMovementRepository
    {
        Task InsertMovementAsync(Domain.Entities.Movement Movement);

        Task<decimal> GetBalance(string id);
    }
}
