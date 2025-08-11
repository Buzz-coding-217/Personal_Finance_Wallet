namespace FinanceWallet.Domain.Entities
{
    public class Spending
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public System.DateTime Date { get; set; } = System.DateTime.UtcNow;
    }
}
