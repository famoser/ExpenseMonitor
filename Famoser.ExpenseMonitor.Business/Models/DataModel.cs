using System.Collections.ObjectModel;

namespace Famoser.ExpenseMonitor.Business.Models
{
    public class DataModel
    {
        public DataModel()
        {
            Collections = new ObservableCollection<ExpenseCollectionModel>();
            DeletedCollections = new ObservableCollection<ExpenseCollectionModel>();
        }

        public ObservableCollection<ExpenseCollectionModel> Collections { get; set; }
        public ObservableCollection<ExpenseCollectionModel> DeletedCollections { get; set; }
    }
}
