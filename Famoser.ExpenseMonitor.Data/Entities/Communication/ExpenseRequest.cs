using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;
using Famoser.ExpenseMonitor.Data.Enum;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class ExpenseRequest : BaseRequest
    {
        public ExpenseRequest(PossibleActions action, Guid guid) : base(action, guid)
        { }

        [DataMember]
        public List<ExpenseEntity> Notes { get; set; }

        [DataMember]
        public Guid NoteCollectionGuid { get; set; }
    }
}
