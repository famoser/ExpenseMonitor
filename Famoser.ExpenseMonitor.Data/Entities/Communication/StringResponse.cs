using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Entities.Communication.Base;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication
{
    [DataContract]
    public class StringReponse : BaseResponse
    {
        [DataMember]
        public string Response { get; set; }
    }
}
