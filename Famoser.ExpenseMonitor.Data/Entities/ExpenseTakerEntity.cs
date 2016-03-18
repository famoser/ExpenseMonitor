using System;
using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities
{
    [DataContract]
    public class ExpenseTakerEntity
    {
        [DataMember]
        public Guid Guid;
    }
}
