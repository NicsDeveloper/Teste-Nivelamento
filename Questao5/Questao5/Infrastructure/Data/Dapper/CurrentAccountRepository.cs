using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Application.Interfaces;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Data
{
    public class CurrentAccountRepository : ICurrentAccountRepository
    {
        private readonly DatabaseConfig _config;

        public CurrentAccountRepository(DatabaseConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<CurrentAccount> GetCurrentAccountByIdAsync(string idConta)
        {
            if (string.IsNullOrWhiteSpace(idConta))
                throw new ArgumentException("The account ID cannot be null or empty.", nameof(idConta));

            try
            {
                using var connection = new SqliteConnection(_config.Name);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        CurrentAccountId AS CurrentAccountId, 
                        number, 
                        name, 
                        active 
                    FROM CurrentAccount 
                    WHERE CurrentAccountId = @id";

                return await connection.QueryFirstOrDefaultAsync<CurrentAccount>(sql, new { id = idConta });
            }
            catch (SqliteException ex)
            {
                // Log the exception (you can use a logging framework here)
                throw new Exception("An error occurred while accessing the database.", ex);
            }
        }
    }
}
