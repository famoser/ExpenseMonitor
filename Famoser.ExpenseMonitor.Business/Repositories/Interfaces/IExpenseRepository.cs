using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Business.Models;

namespace Famoser.ExpenseMonitor.Business.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<ObservableCollection<ExpenseCollectionModel>> GetCollections();
        ObservableCollection<ExpenseCollectionModel> GetExampleCollections();
        Task<bool> SyncExpenses();

        Task<bool> Save(ExpenseModel nm);
        Task<bool> Delete(ExpenseModel nm);

        Task<bool> Save(ExpenseCollectionModel nm);
        Task<bool> Delete(ExpenseCollectionModel nm);
    }
}
