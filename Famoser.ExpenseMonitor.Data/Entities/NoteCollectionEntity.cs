using System;
using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities
{
    [DataContract]
    public class NoteCollectionEntity
    {
        [DataMember]
        public Guid Guid;

        [DataMember]
        public string Name;

        [DataMember]
        public DateTime CreateTime;
    }
}
