using System;
using Famoser.ExpenseMonitor.Business.Enums;
using GalaSoft.MvvmLight;

namespace Famoser.ExpenseMonitor.Business.Models
{
    public class SyncModel : ObservableObject
    {
        private Guid _guid;
        public Guid Guid
        {
            get { return _guid; }
            set { Set(ref _guid, value); }
        }
        
        public PendingAction PendingAction { get; set; }
    }
}
