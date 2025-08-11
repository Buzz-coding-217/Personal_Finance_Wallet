using System;

namespace FinanceWallet.Application.DTOs
{
    public record CreateSpendingDto(decimal Amount, string Description, string Category, DateTime Date);
}
