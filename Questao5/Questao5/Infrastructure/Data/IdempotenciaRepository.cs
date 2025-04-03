using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Application.Interfaces;
using Questao5.Infrastructure.Sqlite;
using Newtonsoft.Json;

namespace Questao5.Infrastructure.Data
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly DatabaseConfig _config;

        public IdempotenciaRepository(DatabaseConfig config)
        {
            _config = config;
        }

        public async Task<bool> ExistsAsync(string idempotencia)
        {
            using var connection = new SqliteConnection(_config.Name);
            await connection.OpenAsync();
            var sql = "SELECT COUNT(1) FROM idempotencia WHERE chave_idempotencia = @idempotencia";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { idempotencia });
            return count > 0;
        }

        public async Task<object> GetResultadoAsync(string idempotencia)
        {
            using var connection = new SqliteConnection(_config.Name);
            await connection.OpenAsync();
            var sql = "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @idempotencia";
            // Considerando que o resultado foi salvo como JSON
            var json = await connection.QueryFirstOrDefaultAsync<string>(sql, new { idempotencia });
            if (json != null)
            {
                return JsonConvert.DeserializeObject(json)!;
            }
            else
            {
                return null!;
            }
        }

        public async Task SaveAsync(string idempotencia, object resultado)
        {
            using var connection = new SqliteConnection(_config.Name);
            await connection.OpenAsync();
            var sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@idempotencia, '', @resultado)";
            var resultadoJson = JsonConvert.SerializeObject(resultado);
            await connection.ExecuteAsync(sql, new { idempotencia, resultado = resultadoJson });
        }
    }
}
