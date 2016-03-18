using System.Runtime.Serialization;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication.Base
{
    [DataContract]
    public class BaseResponse
    {
        public bool IsSuccessfull
        {
            get { return ErrorMessage == null; }
        }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
