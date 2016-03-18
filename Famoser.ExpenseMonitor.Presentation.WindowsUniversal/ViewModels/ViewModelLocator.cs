using Famoser.ExpenseMonitor.Data.Services;
using Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Services;
using Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Services.Mocks;
using Famoser.ExpenseMonitor.View.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public partial class ViewModelLocator : BaseViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Create design time view services and models
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<INavigationService, MockNavigationService>();
            }
            else
            {

                var navigationService = NavigationHelper.CreateNavigationService();
                SimpleIoc.Default.Register(() => navigationService);
            }
        }
    }
}
