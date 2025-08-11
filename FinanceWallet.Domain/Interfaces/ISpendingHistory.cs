using System.Collections.Generic;
using FinanceWallet.Domain.Entities;

namespace FinanceWallet.Domain.Interfaces
{
    public interface ISpendingHistory
    {
        Spending Add(Spending spending);
        IEnumerable<Spending> GetAll();
        IEnumerable<Spending> GetByCategory(string category);
        Spending GetById(int id);
        void Delete(int id);
        void ClearAll();
    }
}
