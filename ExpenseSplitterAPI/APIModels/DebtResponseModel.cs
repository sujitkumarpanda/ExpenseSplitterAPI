namespace ExpenseSplitterAPI.APIModels
{
    public class DebtResponseModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ExpenseId { get; set; }
        public int OwedByUserId { get; set; }
        public int OwedToUserId { get; set; }
        public decimal Amount { get; set; }
    }
}
