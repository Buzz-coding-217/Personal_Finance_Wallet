namespace FinanceWallet.Domain.Entities
{
    public class Budget
    {
        public decimal MonthlyLimit { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
