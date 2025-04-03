namespace Questao5.Application.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<bool> ExistsAsync(string idempotencia);
        Task<object> GetResultadoAsync(string idempotencia);
        Task SaveAsync(string idempotencia, object resultado);
    }
}
