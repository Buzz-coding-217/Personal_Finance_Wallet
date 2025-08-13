using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using FinanceWallet.Domain.Entities;
using FinanceWallet.Domain.Interfaces;
using FinanceWallet.Domain.Exceptions;

namespace FinanceWallet.Infrastructure.Data
{
    public class SpendingQueryRepository : ISpendingQueryRepository
    {
        private readonly string connectionString;

        public SpendingQueryRepository(string dbFilePath)
        {
            connectionString = $"Data Source={dbFilePath}";
        }

        public IEnumerable<Spending> GetAll()
        {
            try
            {
                var spendings = new List<Spending>();

                using var connection = new SqliteConnection(connectionString);
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
            catch (SqliteException ex)
            {
                throw new DataAccessException("Failed to retrieve all spendings.", ex);
            }
        }

        public Spending GetById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
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
            catch (SqliteException ex)
            {
                throw new DataAccessException($"Failed to retrieve spending with Id {id}.", ex);
            }
        }

        public IEnumerable<Spending> GetByCategory(string category)
        {
            try
            {
                var spendings = new List<Spending>();

                using var connection = new SqliteConnection(connectionString);
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
            catch (SqliteException ex)
            {
                throw new DataAccessException($"Failed to retrieve spendings by category '{category}'.", ex);
            }
        }
    }
}
