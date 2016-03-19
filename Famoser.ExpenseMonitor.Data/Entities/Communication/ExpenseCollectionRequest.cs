using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;
using Famoser.ExpenseMonitor.Data.Enum;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    public class ExpenseCollectionRequest :  BaseRequest
    {
        public ExpenseCollectionRequest(PossibleActions action, Guid expenseTakerGuid) : base(action, expenseTakerGuid)
        {
        }
        
        [DataMember]
        public List<ExpenseCollectionEntity> ExpenseCollections { get; set; }
    }
}
