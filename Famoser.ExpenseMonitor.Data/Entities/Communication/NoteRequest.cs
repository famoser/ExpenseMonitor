using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;
using Famoser.ExpenseMonitor.Data.Enum;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class NoteRequest : BaseRequest
    {
        public NoteRequest(PossibleActions action, Guid guid) : base(action, guid)
        { }

        [DataMember]
        public List<NoteEntity> Notes { get; set; }

        [DataMember]
        public Guid NoteCollectionGuid { get; set; }
    }
}
