using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using FinanceWallet.Domain.Entities;
using FinanceWallet.Domain.Interfaces;

namespace FinanceWallet.Infrastructure.Data
{
    public class SqliteSpendingCommandRepository : ISpendingCommandRepository
    {
        private readonly string _connectionString;

        public SqliteSpendingCommandRepository(string dbFilePath)
        {
            _connectionString = $"Data Source={dbFilePath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Spendings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Amount REAL NOT NULL,
                    Description TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Date TEXT NOT NULL
                );
            ";
            cmd.ExecuteNonQuery();
        }

        public Spending Add(Spending spending)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Spendings (Amount, Description, Category, Date)
                VALUES ($amount, $description, $category, $date);
                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$amount", spending.Amount);
            cmd.Parameters.AddWithValue("$description", spending.Description);
            cmd.Parameters.AddWithValue("$category", spending.Category);
            cmd.Parameters.AddWithValue("$date", spending.Date.ToString("o"));

            var id = (long)cmd.ExecuteScalar();
            spending.Id = (int)id;

            return spending;
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Spendings WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void ClearAll()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Spendings";
            cmd.ExecuteNonQuery();
        }
    }
}
