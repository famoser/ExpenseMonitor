using System;
using System.Collections.ObjectModel;
using System.Linq;
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

                NewExpenseDescription = "my description is longer than the place avaliable";
                NewExpenseAmount = 5.2;
                NewExpenseDate = DateTime.Now;
            }
            else
            {
                Initialize();
                SetExpenseDefaults();
            }
            _removeExpenseCollection = new RelayCommand<ExpenseCollectionModel>(RemoveExpenseCollection, CanRemoveExpenseCollection);
            _saveExpenseCollection = new RelayCommand<ExpenseCollectionModel>(SaveExpenseCollection, CanSaveExpenseCollection);
            _addExpenseCollectionCommand = new RelayCommand(AddExpenseCollection, () => CanAddExpenseCollection);
            _useExpenseAsTemplateCommand = new RelayCommand<ExpenseModel>(UseExpenseAsTemplate);

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
            ActiveCollection = ExpenseCollections.FirstOrDefault();
            RaisePropertyChanged(() => TotalExpenseAmount);

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
            RaisePropertyChanged(() => TotalExpenseAmount);

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

        private string _newExpenseDescription;
        public string NewExpenseDescription
        {
            get { return _newExpenseDescription; }
            set
            {
                if (Set(ref _newExpenseDescription, value))
                    _addExpenseCommand.RaiseCanExecuteChanged();
            }
        }

        private double? _newExpenseAmount;
        public double? NewExpenseAmount
        {
            get { return _newExpenseAmount; }
            set
            {
                if (Set(ref _newExpenseAmount, value))
                    _addExpenseCommand.RaiseCanExecuteChanged();
            }
        }
        
        public double? TotalExpenseAmount
        {
            get { return ActiveCollection?.Expenses.Sum(e => e.Amount); }
        }

        private DateTime _newExpenseDate;
        public DateTime NewExpenseDate
        {
            get { return _newExpenseDate; }
            set
            {
                if (Set(ref _newExpenseDate, value))
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

        public bool CanAddExpense => !string.IsNullOrEmpty(_newExpenseDescription) && NewExpenseAmount.HasValue;

        private async void AddExpense()
        {
            if (NewExpenseAmount.HasValue)
            {
                var newExpense = new ExpenseModel()
                {
                    Description = NewExpenseDescription,
                    Guid = Guid.NewGuid(),
                    CreateTime = NewExpenseDate,
                    Amount = NewExpenseAmount.Value,
                    ExpenseCollection = ActiveCollection
                };
                await _expenseRepository.Save(newExpense);
                RaisePropertyChanged(() => TotalExpenseAmount);

                SetExpenseDefaults();
                Messenger.Default.Send(Messages.ExpenseChanged);
            }
        }

        private readonly RelayCommand<ExpenseModel> _useExpenseAsTemplateCommand;
        public ICommand AddExpenseAsTemplateCommand => _useExpenseAsTemplateCommand;

        private void UseExpenseAsTemplate(ExpenseModel model)
        {
            NewExpenseAmount = model.Amount;
            NewExpenseDescription = model.Description;
            NewExpenseDate = DateTime.Now;
        }

        private void SetExpenseDefaults()
        {
            NewExpenseDate = DateTime.Now;
            var lastExpense = ActiveCollection?.Expenses.FirstOrDefault();
            if (lastExpense != null)
            {
                NewExpenseDescription = lastExpense.Description;
                NewExpenseAmount = lastExpense.Amount;
            }
            else
            {
                NewExpenseAmount = null;
                NewExpenseDescription = null;
            }
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
        public ICommand SaveCommand => _save;

        private async void Save(ExpenseModel expense)
        {
            await _expenseRepository.Save(expense);
            RaisePropertyChanged(() => TotalExpenseAmount);
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
            RaisePropertyChanged(() => TotalExpenseAmount);
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
            RaisePropertyChanged(() => TotalExpenseAmount);
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
            set {
                if (Set(ref _activeCollection, value))
                {
                    RaisePropertyChanged(() => TotalExpenseAmount);
                }
            }
        }
    }
}