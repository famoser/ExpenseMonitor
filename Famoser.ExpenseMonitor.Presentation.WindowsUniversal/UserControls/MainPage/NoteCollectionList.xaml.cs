﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Famoser.ExpenseMonitor.View.Enums;
using GalaSoft.MvvmLight.Messaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.UserControls.MainPage
{
    public sealed partial class NoteCollectionList : UserControl
    {
        public NoteCollectionList()
        {
            this.InitializeComponent();
        }

        private void ListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var model = sender as NoteCollectionList;
            Messenger.Default.Send(model, Messages.Select);
        }
    }
}
