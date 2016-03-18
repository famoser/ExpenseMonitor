using Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Pages;
using Famoser.ExpenseMonitor.View.Enums;
using GalaSoft.MvvmLight.Views;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Services
{
    public class NavigationHelper
    {
        public static INavigationService CreateNavigationService()
        {
            var navigationService = new CustomNavigationService();

            navigationService.Implementation.Configure(PageKeys.MainPage.ToString(), typeof(MainPage));
            navigationService.Implementation.Configure(PageKeys.ConnectPage.ToString(), typeof(ConnectPage));

            return navigationService;
        }
    }
}
