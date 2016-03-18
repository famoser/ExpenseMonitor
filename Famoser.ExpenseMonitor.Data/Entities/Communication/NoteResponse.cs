using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class NoteResponse : BaseResponse
    {
        public NoteResponse()
        {
            Notes = new List<NoteEntity>();
        }

        [DataMember]
        public List<NoteEntity> Notes { get; set; }
    }
}
