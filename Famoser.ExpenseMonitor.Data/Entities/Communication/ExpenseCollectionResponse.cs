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
            ExpenseCollections = new List<ExpenseCollectionEntity>();
        }

        [DataMember]
        public List<ExpenseCollectionEntity> ExpenseCollections { get; set; }
    }
}
