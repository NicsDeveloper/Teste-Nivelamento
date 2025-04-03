using Questao5.Domain.Entities;
using System.Threading.Tasks;

namespace Questao5.Application.Interfaces
{
    public interface ICurrentAccountRepository
    {
        Task<CurrentAccount> GetCurrentAccountByIdAsync(string idConta);
    }
}
