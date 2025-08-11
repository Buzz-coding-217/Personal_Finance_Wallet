using System.Collections.Generic;
using FinanceWallet.Domain.Entities;

namespace FinanceWallet.Domain.Interfaces
{

    // For queries (reads)
    public interface ISpendingQueryRepository
    {
        IEnumerable<Spending> GetAll();
        Spending GetById(int id);
        IEnumerable<Spending> GetByCategory(string category);
    }
}
