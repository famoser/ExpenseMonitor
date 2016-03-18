using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Data.Entities.Communication;

namespace Famoser.ExpenseMonitor.Data.Services
{
    public interface IDataService
    {
        Task<BooleanResponse> PostExpense(ExpenseRequest request);
        Task<BooleanResponse> PostExpenseCollection(ExpenseCollectionRequest request);
        Task<ExpenseResponse> GetExpense(ExpenseRequest request);
        Task<ExpenseCollectionResponse> GetExpenseCollections(ExpenseCollectionRequest request);
    }
}
