using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;
using Famoser.ExpenseMonitor.Data.Enum;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    public class ExpenseCollectionRequest :  BaseRequest
    {
        public ExpenseCollectionRequest(PossibleActions action, Guid noteTakerGuid) : base(action, noteTakerGuid)
        {
        }
        
        [DataMember]
        public List<ExpenseCollectionEntity> NoteCollections { get; set; }
    }
}
