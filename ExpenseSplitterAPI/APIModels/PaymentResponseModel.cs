namespace ExpenseSplitterAPI.APIModels
{
    public class PaymentResponseModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
    }
}
