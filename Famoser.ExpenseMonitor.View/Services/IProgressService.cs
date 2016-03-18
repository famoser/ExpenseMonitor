using Famoser.ExpenseMonitor.View.Enums;

namespace Famoser.ExpenseMonitor.View.Services
{
    public interface IProgressService
    {
        void ShowProgress(ProgressKeys key);
        void HideProgress(ProgressKeys key);
    }
}
