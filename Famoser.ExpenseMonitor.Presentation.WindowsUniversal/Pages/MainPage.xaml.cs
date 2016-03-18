using System;
using System.ComponentModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Famoser.ExpenseMonitor.View.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, ev) =>
            {
                if (!ev.Handled)
                {
                    if (EditCollectionGrid.Visibility == Visibility.Visible)
                    {
                        ev.Handled = true;
                        EditCollectionGrid.Visibility = Visibility.Collapsed;
                    }
                    else if (ExpenseCollectionsOverview.Visibility == Visibility.Visible)
                    {
                        ev.Handled = true;
                        UIElement_OnTapped();
                    }
                }
            };
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.RefreshCommand.CanExecute(null))
                ViewModel.RefreshCommand.Execute(null);
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "ActiveCollection")
            {
                if (ExpenseCollectionsOverview.Visibility == Visibility.Visible)
                    UIElement_OnTapped(null, null);
            }
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var vm = DataContext as MainViewModel;
                var tb = sender as TextBox;
                if (tb == AddNewExpenseCollectionTextBox)
                {
                    if (vm?.AddExpenseCollectionCommand.CanExecute(null) == true)
                        vm.AddExpenseCollectionCommand.Execute(null);
                }
                else if (tb == AddNewExpenseTextBox)
                {
                    if (vm?.AddExpenseCommand.CanExecute(null) == true)
                        vm.AddExpenseCommand.Execute(null);
                }
            }
        }
        
        private void EditCollectionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EditCollectionGrid.Visibility = EditCollectionGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Liste wirklich löschen? Dieser Schritt kann nicht rückgängig gemacht werden", "Liste löschen");
            dialog.Commands.Add(new UICommand("abbrechen"));
            dialog.Commands.Add(new UICommand("löschen", command =>
            {
                if (ViewModel.RemoveExpenseCollectionCommand.CanExecute(ViewModel.ActiveCollection))
                    ViewModel.RemoveExpenseCollectionCommand.Execute(ViewModel.ActiveCollection);
                EditCollectionGrid.Visibility = Visibility.Collapsed;
            }));
            dialog.CancelCommandIndex = 0;
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }

        //private void ListsAppbar_OnTapped(object sender, TappedRoutedEventArgs e)
        //{
        //    MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        //}


        private void UIElement_OnTapped(object sender = null, TappedRoutedEventArgs e = null)
        {
            if (ExpenseCollectionsOverview.Visibility == Visibility.Visible)
            {
                ExpenseCollectionsOverview.Visibility = Visibility.Collapsed;
                ActiveExpenseCollection.Visibility = Visibility.Visible;
            }
            else
            {
                ExpenseCollectionsOverview.Visibility = Visibility.Visible;
                ActiveExpenseCollection.Visibility = Visibility.Collapsed;
            }
        }
    }
}
