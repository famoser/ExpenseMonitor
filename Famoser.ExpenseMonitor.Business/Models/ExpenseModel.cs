using System;
using Newtonsoft.Json;

namespace Famoser.ExpenseMonitor.Business.Models
{
    public class ExpenseModel : SyncModel
    {
        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private double _amount;
        public double Amount
        {
            get { return _amount; }
            set { Set(ref _amount, value); }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { Set(ref _createTime, value); }
        }

        [JsonIgnore]
        public ExpenseCollectionModel ExpenseCollection { get; set; }
    }
}
