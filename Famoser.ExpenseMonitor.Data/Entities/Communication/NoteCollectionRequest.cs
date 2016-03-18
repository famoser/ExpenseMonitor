using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;
using Famoser.ExpenseMonitor.Data.Enum;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    public class NoteCollectionRequest :  BaseRequest
    {
        public NoteCollectionRequest(PossibleActions action, Guid noteTakerGuid) : base(action, noteTakerGuid)
        {
        }
        
        [DataMember]
        public List<NoteCollectionEntity> NoteCollections { get; set; }
    }
}
