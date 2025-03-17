namespace ExpenseSplitterAPI.APIModels
{
    public class ExpenseCreateModel
    {
        public int GroupId { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public int PayerId { get; set; }
    }
}
