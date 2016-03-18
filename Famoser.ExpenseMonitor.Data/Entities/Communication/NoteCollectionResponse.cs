using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class NoteCollectionResponse : BaseResponse
    {
        public NoteCollectionResponse()
        {
            NoteCollections = new List<NoteCollectionEntity>();
        }

        [DataMember]
        public List<NoteCollectionEntity> NoteCollections { get; set; }
    }
}
