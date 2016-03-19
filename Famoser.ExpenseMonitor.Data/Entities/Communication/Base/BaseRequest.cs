using System;
using System.Runtime.Serialization;
using Famoser.ExpenseMonitor.Data.Enum;
using Famoser.FrameworkEssentials.Logging;

namespace Famoser.ExpenseMonitor.Data.Entities.Communication.Base
{
    [DataContract]
    public class BaseRequest
    {
        public BaseRequest(PossibleActions action, Guid expenseTakerGuid)
        {
            _possibleAction = action;
            ExpenseTakerGuid = expenseTakerGuid;
        }

        private readonly PossibleActions _possibleAction;

        [DataMember]
        public Guid ExpenseTakerGuid { get; }

        [DataMember]
        public string Action
        {
            get
            {
                if (_possibleAction == PossibleActions.Delete)
                    return "delete";
                if (_possibleAction == PossibleActions.AddOrUpdate)
                    return "addorupdate";
                if (_possibleAction == PossibleActions.Get)
                    return "get";
                LogHelper.Instance.Log(LogLevel.WtfAreYouDoingError, this, "Unknown Possible Action used!");
                return "";
            }
        }
    }
}
