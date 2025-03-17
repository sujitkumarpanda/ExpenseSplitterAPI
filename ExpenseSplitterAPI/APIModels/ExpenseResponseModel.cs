namespace ExpenseSplitterAPI.APIModels
{
    public class ExpenseResponseModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public int PayerId { get; set; }
        public string PayerName { get; set; } = null!;
    }
}
