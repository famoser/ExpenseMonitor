using System;
using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities
{
    [DataContract]
    public class ExpenseCollectionEntity
    {
        [DataMember]
        public Guid Guid;

        [DataMember]
        public string Name;

        [DataMember]
        public DateTime CreateTime;
    }
}
