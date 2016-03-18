using System;
using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities
{
    [DataContract]
    public class NoteTakerEntity
    {
        [DataMember]
        public Guid Guid;
    }
}
