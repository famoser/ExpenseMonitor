using System.Collections.Generic;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class ExpenseResponse : BaseResponse
    {
        public ExpenseResponse()
        {
            Notes = new List<ExpenseEntity>();
        }

        [DataMember]
        public List<ExpenseEntity> Notes { get; set; }
    }
}
