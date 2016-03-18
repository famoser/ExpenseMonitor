using System;
using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities
{
    [DataContract]
    public class NoteEntity
    {
        [DataMember]
        public Guid Guid;

        [DataMember]
        public string Content;

        [DataMember]
        public DateTime CreateTime;

        [DataMember]
        public string IsCompleted
        {
            set
            {
                if (value == "1" || value == "true" || value == "True")
                    IsCompletedBool = true;
            }
            get { return IsCompletedBool.ToString(); }
        }

        public bool IsCompletedBool;
    }
}
