using System;
using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities
{
    [DataContract]
    public class ExpenseEntity
    {
        [DataMember]
        public Guid Guid;

        [DataMember]
        public string Content;

        [DataMember]
        public DateTime CreateTime;

        [DataMember]
        public double Amount;
    }
}
