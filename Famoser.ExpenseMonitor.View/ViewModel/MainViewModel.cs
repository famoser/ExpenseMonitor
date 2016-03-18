using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Business.Repositories.Interfaces;
using Famoser.ExpenseMonitor.View.Enums;
using Famoser.ExpenseMonitor.View.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;

namespace Famoser.ExpenseMonitor.View.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IExpenseRepository _expenseRepository;
        private IProgressService _progressService;
        private INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IExpenseRepository expenseRepository, IProgressService progressService, INavigationService navigationService)
        {
            _expenseRepository = expenseRepository;
            _progressService = progressService;
            _navigationService = navigationService;

            _refreshCommand = new RelayCommand(Refresh, () => CanRefresh);
            _addExpenseCommand = new RelayCommand(AddExpense, () => CanAddExpense);
            _removeExpense = new RelayCommand<ExpenseModel>(RemoveExpense);
            _save = new RelayCommand<ExpenseModel>(Save);

            if (IsInDesignMode)
            {
                ExpenseCollections = expenseRepository.GetExampleCollections();
                ActiveCollection = ExpenseCollections[0];
            }
            else
            {
                Initialize();
            }
            _removeExpenseCollection = new RelayCommand<ExpenseCollectionModel>(RemoveExpenseCollection, CanRemoveExpenseCollection);
            _saveExpenseCollection = new RelayCommand<ExpenseCollectionModel>(SaveExpenseCollection, CanSaveExpenseCollection);
            _addExpenseCollectionCommand = new RelayCommand(AddExpenseCollection, () => CanAddExpenseCollection);

            Messenger.Default.Register<ExpenseCollectionModel>(this, Messages.Select, EvaluateSelectMessage);
        }

        private void EvaluateSelectMessage(ExpenseCollectionModel obj)
        {
            ActiveCollection = obj;
        }

        //enable for debug purposes
        private bool _isInitializing;
        private async void Initialize()
        {
            _progressService.ShowProgress(ProgressKeys.InitializingApplication);
            _isInitializing = true;
            _refreshCommand.RaiseCanExecuteChanged();

            ExpenseCollections = await _expenseRepository.GetCollections();
            ActiveCollection = ExpenseCollections[0];

            _isInitializing = false;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.HideProgress(ProgressKeys.InitializingApplication);
        }

        private bool _isSyncing;
        private async Task SyncExpenses()
        {
            _progressService.ShowProgress(ProgressKeys.SyncingExpenses);
            _isSyncing = true;
            _refreshCommand.RaiseCanExecuteChanged();

            await _expenseRepository.SyncExpenses();
            Messenger.Default.Send(Messages.ExpenseChanged);

            _isSyncing = false;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.HideProgress(ProgressKeys.SyncingExpenses);
        }

        private readonly RelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;

        public bool CanRefresh => !_isInitializing && !_isSyncing;

        private async void Refresh()
        {
            await SyncExpenses();
        }

        private string _newExpense;
        public string NewExpense
        {
            get { return _newExpense; }
            set
            {
                if (Set(ref _newExpense, value))
                    _addExpenseCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newExpenseCollection;
        public string NewExpenseCollection
        {
            get { return _newExpenseCollection; }
            set
            {
                if (Set(ref _newExpenseCollection, value))
                    _addExpenseCollectionCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly RelayCommand _addExpenseCommand;
        public ICommand AddExpenseCommand => _addExpenseCommand;

        public bool CanAddExpense => !string.IsNullOrEmpty(_newExpense);

        private async void AddExpense()
        {
            var newExpense = new ExpenseModel()
            {
                Description = NewExpense,
                Guid = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                ExpenseCollection = ActiveCollection
            };
            NewExpense = "";
            await _expenseRepository.Save(newExpense);
            Messenger.Default.Send(Messages.ExpenseChanged);
        }

        private readonly RelayCommand _addExpenseCollectionCommand;
        public ICommand AddExpenseCollectionCommand => _addExpenseCollectionCommand;

        public bool CanAddExpenseCollection => !string.IsNullOrEmpty(NewExpenseCollection);

        private async void AddExpenseCollection()
        {
            var newExpenseCollection = new ExpenseCollectionModel()
            {
                Guid = Guid.NewGuid(),
                Name = NewExpenseCollection,
                CreateTime = DateTime.Now
            };
            NewExpenseCollection = "";
            await _expenseRepository.Save(newExpenseCollection);
            ActiveCollection = newExpenseCollection;
        }

        private readonly RelayCommand<ExpenseModel> _save;
        public ICommand SaveCommand { get { return _save; } }

        private async void Save(ExpenseModel expense)
        {
            await _expenseRepository.Save(expense);
            Messenger.Default.Send(Messages.ExpenseChanged);
        }

        private readonly RelayCommand<ExpenseCollectionModel> _removeExpenseCollection;
        public ICommand RemoveExpenseCollectionCommand => _removeExpenseCollection;

        public bool CanRemoveExpenseCollection(ExpenseCollectionModel model)
        {
            return ExpenseCollections.Count > 1;
        }

        private async void RemoveExpenseCollection(ExpenseCollectionModel model)
        {
            if (model == ActiveCollection)
            {
                var index = ExpenseCollections.IndexOf(ActiveCollection);
                if (index == 0)
                    ActiveCollection = ExpenseCollections[1];
                else
                    ActiveCollection = ExpenseCollections[--index];
            }
            await _expenseRepository.Delete(model);
            Messenger.Default.Send(Messages.ExpenseChanged);
        }

        private readonly RelayCommand<ExpenseCollectionModel> _saveExpenseCollection;
        public ICommand SaveExpenseCollectionCommand => _saveExpenseCollection;

        public bool CanSaveExpenseCollection(ExpenseCollectionModel model)
        {
            return !string.IsNullOrEmpty(model?.Name);
        }

        private async void SaveExpenseCollection(ExpenseCollectionModel model)
        {
            await _expenseRepository.Save(model);
        }

        private readonly RelayCommand<ExpenseModel> _removeExpense;
        public ICommand RemoveExpenseCommand => _removeExpense;

        private async void RemoveExpense(ExpenseModel expense)
        {
            await _expenseRepository.Delete(expense);
            Messenger.Default.Send(Messages.ExpenseChanged);
        }

        private ObservableCollection<ExpenseCollectionModel> _expenseCollections;
        public ObservableCollection<ExpenseCollectionModel> ExpenseCollections
        {
            get { return _expenseCollections; }
            set { Set(ref _expenseCollections, value); }
        }

        private ExpenseCollectionModel _activeCollection;
        public ExpenseCollectionModel ActiveCollection
        {
            get { return _activeCollection; }
            set { Set(ref _activeCollection, value); }
        }
    }
}