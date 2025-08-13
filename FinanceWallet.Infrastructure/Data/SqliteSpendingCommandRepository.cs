using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using FinanceWallet.Domain.Entities;
using FinanceWallet.Domain.Interfaces;
using FinanceWallet.Domain.Exceptions;

namespace FinanceWallet.Infrastructure.Data
{
    public class SpendingCommandRepository : ISpendingCommandRepository
    {
        private readonly string connectionString;

        public SpendingCommandRepository(string dbFilePath)
        {
            connectionString = $"Data Source={dbFilePath}";
            try
            {
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to initialize the database.", ex);
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
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
            catch (SqliteException ex)
            {
                throw new DataAccessException("Database initialization error.", ex);
            }
        }

        public void Add(Spending spending)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
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

                var id = cmd.ExecuteNonQuery();
                spending.Id = (int)id;
            }
            catch (SqliteException ex)
            {
                throw new DataAccessException("Failed to add spending.", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM Spendings WHERE Id = $id";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                throw new DataAccessException($"Failed to delete spending with Id {id}.", ex);
            }
        }

        public void ClearAll()
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM Spendings";
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                throw new DataAccessException("Failed to clear all spendings.", ex);
            }
        }
    }
}