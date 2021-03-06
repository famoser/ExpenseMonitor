﻿using System;
using GalaSoft.MvvmLight.Views;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Services.Mocks
{
    public class MockNavigationService : INavigationService
    {
        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            throw new NotImplementedException();
        }

        public string CurrentPageKey { get; }
    }
}
