using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Application.Interfaces;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Data
{
    public class MovementRepository : IMovementRepository
    {
        private readonly DatabaseConfig _config;

        public MovementRepository(DatabaseConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<decimal> GetBalance(string id)
        {
            using var connection = new SqliteConnection(_config.Name);
            await connection.OpenAsync();

            const string sql = @"
            SELECT IFNULL(SUM(CASE WHEN movementType = 'C' THEN value ELSE 0 END), 0) -
                   IFNULL(SUM(CASE WHEN movementType = 'D' THEN value ELSE 0 END), 0) AS Balance
            FROM Movement
            WHERE currentAccountId = @CurrentAccountId";

            return await connection.ExecuteScalarAsync<decimal>(sql, new { CurrentAccountId = id });
        }

        public async Task InsertMovementAsync(Movement movement)
        {
            if (movement == null)
                throw new ArgumentNullException(nameof(movement), "The movement object cannot be null.");

            try
            {
                using var connection = new SqliteConnection(_config.Name);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO Movement (movementId, currentAccountId, movementData, movementType, value)
                    VALUES (@MovementId, @CurrentAccountId, @MovementData, @MovementType, @Value)";

                await connection.ExecuteAsync(sql, movement);
            }
            catch (SqliteException ex)
            {
                // Log the exception (you can use a logging framework here)
                throw new Exception("An error occurred while inserting the movement into the database.", ex);
            }
        }
    }
}
