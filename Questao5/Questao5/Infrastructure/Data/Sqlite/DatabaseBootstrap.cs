using Dapper;
using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND (name = 'CurrentAccount' or name = 'Movement' or name = 'idempotencia');");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && (tableName == "CurrentAccount" || tableName == "Movement" || tableName == "idempotencia"))
                return;

            connection.Execute("CREATE TABLE CurrentAccount ( " +
                               "CurrentAccountId TEXT(37) PRIMARY KEY," +
                               "number INTEGER(10) NOT NULL UNIQUE," +
                               "name TEXT(100) NOT NULL," +
                               "active INTEGER(1) NOT NULL default 0," +
                               "CHECK(active in (0, 1)) " +
                               ");");

            connection.Execute("CREATE TABLE Movement ( " +
                "movementId TEXT(37) PRIMARY KEY," +
                "currentAccountId TEXT(37) NOT NULL," +
                "movementData TEXT(25) NOT NULL," +
                "movementType TEXT(1) NOT NULL," +
                "value REAL NOT NULL," +
                "CHECK(movementType in ('C', 'D')), " +
                "FOREIGN KEY(currentAccountId) REFERENCES CurrentAccount(CurrentAccountId) " +
                ");");

            connection.Execute("CREATE TABLE idempotencia (" +
                               "chave_idempotencia TEXT(37) PRIMARY KEY," +
                               "requisicao TEXT(1000)," +
                               "resultado TEXT(1000));");

            connection.Execute("INSERT INTO CurrentAccount(currentAccountId, number, name, active) VALUES('B6BAFC09-6967-ED11-A567-055DFA4A16C9', 123, 'Katherine Sanchez', 1);");
            connection.Execute("INSERT INTO CurrentAccount(currentAccountId, number, name, active) VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Eva Woodward', 1);");
            connection.Execute("INSERT INTO CurrentAccount(currentAccountId, number, name, active) VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'Tevin Mcconnell', 1);");
            connection.Execute("INSERT INTO CurrentAccount(currentAccountId, number, name, active) VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ameena Lynn', 0);");
            connection.Execute("INSERT INTO CurrentAccount(currentAccountId, number, name, active) VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Jarrad Mckee', 0);");
            connection.Execute("INSERT INTO CurrentAccount(currentAccountId, number, name, active) VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Elisha Simons', 0);");
        }
    }
}
