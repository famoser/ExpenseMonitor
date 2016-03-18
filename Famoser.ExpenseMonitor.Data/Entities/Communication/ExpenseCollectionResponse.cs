using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class ExpenseCollectionResponse : BaseResponse
    {
        public ExpenseCollectionResponse()
        {
            NoteCollections = new List<ExpenseCollectionEntity>();
        }

        [DataMember]
        public List<ExpenseCollectionEntity> NoteCollections { get; set; }
    }
}
