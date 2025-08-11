using System;
using System.Collections.Generic;
using System.Linq;
using FinanceWallet.Application.DTOs;
using FinanceWallet.Domain.Entities;
using FinanceWallet.Domain.Interfaces;

namespace FinanceWallet.Application.Services
{
    public class SpendingService
    {
        private readonly ISpendingCommandRepository commandRepo;
        private readonly ISpendingQueryRepository queryRepo;
        private Budget currentBudget = new Budget();

        public SpendingService(ISpendingCommandRepository commandRepo, ISpendingQueryRepository queryRepo)
        {
            this.commandRepo = commandRepo;
            this.queryRepo = queryRepo;
        }

        public void AddSpending(CreateSpendingDto dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be positive");
            var s = new Spending
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category,
                Date = dto.Date
            };
            commandRepo.Add(s);
        }

        public IEnumerable<Spending> GetAllSpendings() =>
            queryRepo.GetAll().OrderByDescending(s => s.Date);

        public IEnumerable<Spending> GetSpendingsByCategory(string category) =>
            queryRepo.GetByCategory(category ?? string.Empty).OrderByDescending(s => s.Date);

        public decimal GetTotalSpendings() =>
            queryRepo.GetAll().Sum(s => s.Amount);

        public decimal GetTotalForMonth(int year, int month) =>
            queryRepo.GetAll().Where(s => s.Date.Year == year && s.Date.Month == month).Sum(s => s.Amount);

        public void DeleteSpending(int id) =>
            commandRepo.Delete(id);

        public void ClearAllSpendings() =>
            commandRepo.ClearAll();

        public void SetMonthlyBudget(decimal amount, int year, int month)
        {
            if (amount < 0) throw new ArgumentException("Budget cannot be negative");
            currentBudget = new Budget { MonthlyLimit = amount, Year = year, Month = month };
        }

        public Budget GetCurrentBudget() => currentBudget;

        public decimal GetRemainingBudgetForCurrentMonth()
        {
            var spent = GetTotalForMonth(currentBudget.Year, currentBudget.Month);
            return currentBudget.MonthlyLimit - spent;
        }
    }
}
