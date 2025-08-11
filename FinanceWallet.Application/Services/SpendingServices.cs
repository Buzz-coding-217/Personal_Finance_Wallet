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
        private readonly ISpendingHistory _history;
        private Budget _currentBudget = new Budget();

        public SpendingService(ISpendingHistory history)
        {
            _history = history;
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
            return _history.Add(s);
        }

        public IEnumerable<Spending> GetAllSpendings() => _history.GetAll().OrderByDescending(s => s.Date);

        public IEnumerable<Spending> GetSpendingsByCategory(string category) =>
            _history.GetByCategory(category ?? string.Empty).OrderByDescending(s => s.Date);

        public decimal GetTotalSpendings() => _history.GetAll().Sum(s => s.Amount);

        public decimal GetTotalForMonth(int year, int month) =>
            _history.GetAll().Where(s => s.Date.Year == year && s.Date.Month == month).Sum(s => s.Amount);

        public void DeleteSpending(int id) => _history.Delete(id);

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

        public void ClearAllSpendings() => _history.ClearAll();
    }
}
