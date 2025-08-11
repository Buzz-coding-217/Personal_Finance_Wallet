using System;
using FinanceWallet.Application.DTOs;
using FinanceWallet.Application.Services;
using FinanceWallet.Infrastructure.Data;

namespace FinanceWallet.Presentation
{
    class Program
    {
        static void Main()
        {
            var dbPath = "financewallet.db";

            var commandRepo = new SqliteSpendingCommandRepository(dbPath);
            var queryRepo = new SqliteSpendingQueryRepository(dbPath);

            var service = new SpendingService(commandRepo, queryRepo);

            var now = DateTime.UtcNow;
            service.SetMonthlyBudget(1000m, now.Year, now.Month);
            Console.WriteLine($"Monthly budget set to {service.GetCurrentBudget().MonthlyLimit:C} for {now:MMMM yyyy}");

            service.AddSpending(new CreateSpendingDto(45.50m, "Groceries", "Food", now.AddDays(-3)));
            service.AddSpending(new CreateSpendingDto(15m, "Coffee", "Food", now.AddDays(-2)));
            service.AddSpending(new CreateSpendingDto(120m, "New shoes", "Clothing", now.AddDays(-10)));

            Console.WriteLine("\nAll spendings:");
            foreach (var s in service.GetAllSpendings())
                Console.WriteLine($"{s.Id}: {s.Date:yyyy-MM-dd} | {s.Category} | {s.Description} | {s.Amount:C}");

            Console.WriteLine($"\nTotal spendings: {service.GetTotalSpendings():C}");
            Console.WriteLine($"Total this month: {service.GetTotalForMonth(now.Year, now.Month):C}");
            Console.WriteLine($"Remaining budget for month: {service.GetRemainingBudgetForCurrentMonth():C}");

            Console.WriteLine("\nFood spendings:");
            foreach (var s in service.GetSpendingsByCategory("Food"))
                Console.WriteLine($"{s.Date:yyyy-MM-dd} | {s.Description} | {s.Amount:C}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
