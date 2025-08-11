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
        private readonly ISpendingCommandRepository _commandRepo;
        private readonly ISpendingQueryRepository _queryRepo;
        private Budget _currentBudget = new Budget();

        public SpendingService(ISpendingCommandRepository commandRepo, ISpendingQueryRepository queryRepo)
        {
            _commandRepo = commandRepo;
            _queryRepo = queryRepo;
        }

        public Spending AddSpending(CreateSpendingDto dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be positive");
            var s = new Spending
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category,
                Date = dto.Date
            };
            return _commandRepo.Add(s);
        }

        public IEnumerable<Spending> GetAllSpendings() =>
            _queryRepo.GetAll().OrderByDescending(s => s.Date);

        public IEnumerable<Spending> GetSpendingsByCategory(string category) =>
            _queryRepo.GetByCategory(category ?? string.Empty).OrderByDescending(s => s.Date);

        public decimal GetTotalSpendings() =>
            _queryRepo.GetAll().Sum(s => s.Amount);

        public decimal GetTotalForMonth(int year, int month) =>
            _queryRepo.GetAll().Where(s => s.Date.Year == year && s.Date.Month == month).Sum(s => s.Amount);

        public void DeleteSpending(int id) =>
            _commandRepo.Delete(id);

        public void ClearAllSpendings() =>
            _commandRepo.ClearAll();

        public void SetMonthlyBudget(decimal amount, int year, int month)
        {
            if (amount < 0) throw new ArgumentException("Budget cannot be negative");
            _currentBudget = new Budget { MonthlyLimit = amount, Year = year, Month = month };
        }

        public Budget GetCurrentBudget() => _currentBudget;

        public decimal GetRemainingBudgetForCurrentMonth()
        {
            var spent = GetTotalForMonth(_currentBudget.Year, _currentBudget.Month);
            return _currentBudget.MonthlyLimit - spent;
        }
    }
}
