using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using FinanceWallet.Domain.Entities;
using FinanceWallet.Domain.Interfaces;

namespace FinanceWallet.Infrastructure.Data
{
    public class SqliteSpending : ISpendingHistory
    {
        private readonly string _connectionString;

        public SqliteSpending(string dbFilePath)
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

        public IEnumerable<Spending> GetAll()
        {
            var spendings = new List<Spending>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Amount, Description, Category, Date FROM Spendings";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                spendings.Add(new Spending
                {
                    Id = reader.GetInt32(0),
                    Amount = (decimal)reader.GetDouble(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3),
                    Date = DateTime.Parse(reader.GetString(4))
                });
            }
            return spendings;
        }

        public Spending GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Amount, Description, Category, Date FROM Spendings WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Spending
                {
                    Id = reader.GetInt32(0),
                    Amount = (decimal)reader.GetDouble(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3),
                    Date = DateTime.Parse(reader.GetString(4))
                };
            }
            return null;
        }

        public IEnumerable<Spending> GetByCategory(string category)
        {
            var spendings = new List<Spending>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();

            if (string.IsNullOrWhiteSpace(category))
            {
                cmd.CommandText = "SELECT Id, Amount, Description, Category, Date FROM Spendings";
            }
            else
            {
                cmd.CommandText = "SELECT Id, Amount, Description, Category, Date FROM Spendings WHERE LOWER(Category) = LOWER($category)";
                cmd.Parameters.AddWithValue("$category", category.Trim());
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                spendings.Add(new Spending
                {
                    Id = reader.GetInt32(0),
                    Amount = (decimal)reader.GetDouble(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3),
                    Date = DateTime.Parse(reader.GetString(4))
                });
            }
            return spendings;
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
