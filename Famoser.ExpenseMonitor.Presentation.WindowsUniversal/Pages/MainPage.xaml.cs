using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Converters.MainPage;
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
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.RefreshCommand.CanExecute(null))
                ViewModel.RefreshCommand.Execute(null);
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && e.KeyStatus.RepeatCount == 1)
            {
                var tb = sender as TextBox;
                if (tb == AddNewExpenseCollectionTextBox)
                {
                    if (ViewModel?.AddExpenseCollectionCommand.CanExecute(null) == true)
                        ViewModel.AddExpenseCollectionCommand.Execute(null);
                }
                else if (tb == NewExpenseDescriptionTextBox)
                {
                    if (ViewModel?.AddExpenseCommand.CanExecute(null) == true)
                        ViewModel.AddExpenseCommand.Execute(null);
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
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void AmountTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                GoToDescriptionTextBlock();
            }
        }

        private void NewExpenseAmountTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (Regex.IsMatch(tb.Text, @"([0-9])*\.([0-9]){2}"))
            {
                GoToDescriptionTextBlock();
            }
        }

        private void GoToDescriptionTextBlock()
        {
            NewExpenseDescriptionTextBox.Focus(FocusState.Pointer);
        }

        private void NewExpenseDescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var converter = new DoubleToCurrencyConverter();
            NewExpenseAmountTextBox.Text = (string)converter.Convert(ViewModel.NewExpenseAmount, null, null, null);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as ExpenseModel;
            if (ViewModel.AddExpenseAsTemplateCommand.CanExecute(model))
                ViewModel.AddExpenseAsTemplateCommand.Execute(model);
        }

        private void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            UIElement_OnTapped();
        }
    }
}
