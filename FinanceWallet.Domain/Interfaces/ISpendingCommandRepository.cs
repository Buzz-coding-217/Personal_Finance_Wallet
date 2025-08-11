using System.Collections.Generic;
using FinanceWallet.Domain.Entities;

namespace FinanceWallet.Domain.Interfaces
{
    // For commands (writes)
    public interface ISpendingCommandRepository
    {
        void Add(Spending spending);
        void Delete(int id);
        void ClearAll();
    }
}
