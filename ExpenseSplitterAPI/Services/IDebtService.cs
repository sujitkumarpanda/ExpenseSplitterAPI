using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseSplitterAPI.APIModels;

namespace ExpenseSplitterAPI.Services
{
    public interface IDebtService
    {
        Task<List<DebtResponseModel>> GetUserDebts(int userId);
    }
}
