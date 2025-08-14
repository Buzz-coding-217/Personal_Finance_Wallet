using Xunit;
using FinanceWallet.Domain.Entities;
using System.Collections.Generic;

namespace FinanceWallet.Tests
{
    public class TestBudgetExceeded
    {
        [Fact]
        public void BudgetExceeded_ReturnsTrueWhenSpendingIsMore()
        {
            // Arrange
            var budget = new Budget { MonthlyLimit = 100 };
            var spendings = new List<Spending>
            {
                new Spending { Amount = 60 },
                new Spending { Amount = 50 }
            };

            // Act
            decimal total = 0;
            foreach (var s in spendings)
                total += s.Amount;

            bool isExceeded = total > budget.MonthlyLimit;

            // Assert
            Assert.True(isExceeded);
        }
    }
}
