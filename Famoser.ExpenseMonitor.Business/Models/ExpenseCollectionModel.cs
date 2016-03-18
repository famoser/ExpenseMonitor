using System;
using System.Collections.ObjectModel;

namespace Famoser.ExpenseMonitor.Business.Models
{
    public class ExpenseCollectionModel : SyncModel
    {
        public ExpenseCollectionModel()
        {
            Expenses = new ObservableCollection<ExpenseModel>();
            DeletedExpenses = new ObservableCollection<ExpenseModel>();
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { Set(ref _createTime, value); }
        }

        public ObservableCollection<ExpenseModel> Expenses { get; set; }

        public ObservableCollection<ExpenseModel> DeletedExpenses { get; set; }
    }
}
