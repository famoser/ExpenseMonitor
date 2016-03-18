using Famoser.ExpenseMonitor.View.Enums;
using Famoser.ExpenseMonitor.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace Famoser.ExpenseMonitor.View.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ProgressViewModel _viewModel;
        public ProgressService()
        {
            _viewModel = SimpleIoc.Default.GetInstance<ProgressViewModel>();
        }

        public void ShowProgress(ProgressKeys key)
        {
            _viewModel.SetProgressState(key, true);
        }

        public void HideProgress(ProgressKeys key)
        {
            _viewModel.SetProgressState(key, false);
        }
    }
}
