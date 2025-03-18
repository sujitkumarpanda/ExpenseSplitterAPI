using ExpenseSplitterAPI.APIModels;

namespace ExpenseSplitterAPI.Services
{
    public interface IPaymentService
    {
        Task<bool> SettlePayment(PaymentRequestModel request);
    }
}