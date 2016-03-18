using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Business.Converters;
using Famoser.ExpenseMonitor.Business.Enums;
using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Business.Repositories.Interfaces;
using Famoser.ExpenseMonitor.Data.Enum;
using Famoser.ExpenseMonitor.Data.Services;
using Famoser.FrameworkEssentials.Logging;
using Newtonsoft.Json;

namespace Famoser.ExpenseMonitor.Business.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private const int ActiveDataVersion = 1;
        private readonly IDataService _dataService;
        private readonly IStorageService _storageService;

        public ExpenseRepository(IDataService dataService, IStorageService storageService)
        {
            _dataService = dataService;
            _storageService = storageService;
        }

        private UserInformationModel _userInformations;
        private DataModel _dataModel;

        private ExpenseCollectionModel GetExampleCollection(string collName = "to do")
        {
            return new ExpenseCollectionModel()
            {
                Name = collName,
                Guid = Guid.NewGuid(),
                Expenses = new ObservableCollection<ExpenseModel>()
                {
                new ExpenseModel()
                {
                    Guid = Guid.NewGuid(),
                    Description = "Note 1",
                    CreateTime = DateTime.Now,
                    Amount = 13.5,
                },
                new ExpenseModel()
                {
                    Guid = Guid.NewGuid(),
                    Description = "Note 2",
                    CreateTime = DateTime.Now,
                    Amount = 13.5,
                },
                new ExpenseModel()
                {
                    Guid = Guid.NewGuid(),
                    Description = "Note 3",
                    CreateTime = DateTime.Now,
                    Amount = 13.5,
                }
            }
            };
        }

        public async Task<ObservableCollection<ExpenseCollectionModel>> GetCollections()
        {
            try
            {
                if (await RetrieveUserInformationsFromStorage())
                {
                    if (await RetrieveNoteCollectionsFromStorage())
                    {
                        foreach (var noteCollection in _dataModel.Collections)
                        {
                            foreach (var note in noteCollection.Expenses)
                            {
                                note.ExpenseCollection = noteCollection;
                            }
                            foreach (var note in noteCollection.DeletedExpenses)
                            {
                                note.ExpenseCollection = noteCollection;
                            }
                        }
                        foreach (var noteCollection in _dataModel.DeletedCollections)
                        {
                            foreach (var note in noteCollection.Expenses)
                            {
                                note.ExpenseCollection = noteCollection;
                            }
                            foreach (var note in noteCollection.DeletedExpenses)
                            {
                                note.ExpenseCollection = noteCollection;
                            }
                        }
                    }
                    else
                    {
                        _dataModel = new DataModel();
                        await SyncExpenses();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            if (!_dataModel.Collections.Any())
            {
                var coll = new ExpenseCollectionModel()
                {
                    Name = "Meals",
                    Guid = Guid.NewGuid(),
                    PendingAction = PendingAction.AddOrUpdate
                };
                foreach (var note in coll.Expenses)
                {
                    note.ExpenseCollection = coll;
                }

                _dataModel.Collections.Add(coll);
                await SaveExpenseCollectionsToStorage();
            }
            return _dataModel.Collections;
        }

        public ObservableCollection<ExpenseCollectionModel> GetExampleCollections()
        {
            return new ObservableCollection<ExpenseCollectionModel>()
            {
                GetExampleCollection(),
                GetExampleCollection("at home"),
                GetExampleCollection("work")
            };
        }

        public async Task<bool> SyncExpenses()
        {
            try
            {
                //add/update/delete collections
                var collPending = _dataModel.Collections.Where(c => c.PendingAction == PendingAction.AddOrUpdate).ToList();
                if (collPending.Any())
                {
                    var addUpdateRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.AddOrUpdate, collPending);
                    var addUpdateRes = (await _dataService.PostExpenseCollection(addUpdateRequest)).IsSuccessfull;
                    if (addUpdateRes)
                        foreach (var collModel in collPending)
                            collModel.PendingAction = PendingAction.None;
                }

                //delete
                var collDelete = _dataModel.DeletedCollections.Where(c => c.PendingAction == PendingAction.Remove).ToList();
                if (collDelete.Any())
                {
                    var deleteRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.Delete, collDelete);
                    var deleteRes = (await _dataService.PostExpenseCollection(deleteRequest)).IsSuccessfull;
                    if (deleteRes)
                        foreach (var collModel in collDelete)
                        {
                            collModel.PendingAction = PendingAction.None;
                            _dataModel.DeletedCollections.Remove(collModel);
                        }
                }

                //sync
                var syncRequest = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.Get, new List<ExpenseCollectionModel>());
                var syncRequestResult = await _dataService.GetExpenseCollections(syncRequest);
                if (syncRequestResult.IsSuccessfull)
                {
                    //actualize existing / add new
                    foreach (var collectionEntity in syncRequestResult.NoteCollections)
                    {
                        var existingModel = _dataModel.Collections.FirstOrDefault(n => n.Guid == collectionEntity.Guid);
                        if (existingModel == null)
                        {
                            var newModel = ResponseConverter.Instance.Convert(collectionEntity);
                            InsertIntoList(_dataModel.Collections, newModel);
                        }
                        else
                        {
                            ResponseConverter.Instance.WriteValues(collectionEntity, existingModel);
                        }
                    }
                    //remove old
                    var old = _dataModel.Collections.Where(n => syncRequestResult.NoteCollections.All(no => no.Guid != n.Guid)).ToList();
                    foreach (var noteModel in old)
                    {
                        _dataModel.Collections.Remove(noteModel);
                    }
                }


                //add/update/delete all notes
                foreach (var noteCollectionModel in _dataModel.Collections)
                {
                    //add & update
                    var pending = noteCollectionModel.Expenses.Where(n => n.PendingAction == PendingAction.AddOrUpdate).ToList();
                    if (pending.Any())
                    {
                        var addUpdateRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, noteCollectionModel.Guid, PossibleActions.AddOrUpdate, pending);
                        var addUpdateRes = await _dataService.PostExpense(addUpdateRequest);
                        if (addUpdateRes.IsSuccessfull)
                            foreach (var noteModel in pending)
                                noteModel.PendingAction = PendingAction.None;
                    }

                    //removes
                    var removes = noteCollectionModel.DeletedExpenses.Where(n => n.PendingAction == PendingAction.Remove).ToList();
                    if (removes.Any())
                    {
                        var deleteRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid,
                            noteCollectionModel.Guid, PossibleActions.Delete, removes);
                        var delets = await _dataService.PostExpense(deleteRequest);
                        if (delets.IsSuccessfull)
                            foreach (var noteModel in removes)
                            {
                                noteModel.PendingAction = PendingAction.None;
                                noteCollectionModel.DeletedExpenses.Remove(noteModel);
                            }
                    }

                    //sync
                    var getRequest = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, noteCollectionModel.Guid, PossibleActions.Get, new List<ExpenseModel>());
                    var getRequestResult = await _dataService.GetExpense(getRequest);
                    if (getRequestResult.IsSuccessfull)
                    {
                        //actualize existing / add new
                        foreach (var noteEntity in getRequestResult.Notes)
                        {
                            ExpenseModel existingModel = noteCollectionModel.Expenses.FirstOrDefault(n => n.Guid == noteEntity.Guid);
                            if (existingModel == null)
                            {
                                var newModel = ResponseConverter.Instance.Convert(noteEntity);
                                newModel.ExpenseCollection = noteCollectionModel;
                                InsertIntoList(noteCollectionModel.Expenses, newModel);
                            }
                            else
                            {
                                ResponseConverter.Instance.WriteValues(noteEntity, existingModel);
                                if (noteEntity.CreateTime != existingModel.CreateTime)
                                {
                                    noteCollectionModel.Expenses.Remove(existingModel);
                                    InsertIntoList(noteCollectionModel.Expenses, existingModel);
                                }
                            }
                        }
                        //remove old
                        var old = noteCollectionModel.Expenses.Where(n => getRequestResult.Notes.All(no => no.Guid != n.Guid)).ToList();
                        foreach (var noteModel in old)
                        {
                            noteCollectionModel.Expenses.Remove(noteModel);
                        }
                    }
                }

                return await SaveExpenseCollectionsToStorage();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public async Task<bool> Save(ExpenseModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, nm.ExpenseCollection.Guid, PossibleActions.AddOrUpdate,
                    new List<ExpenseModel>() { nm });
                var res = await _dataService.PostExpense(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.AddOrUpdate : PendingAction.None;

                if (nm.ExpenseCollection.Expenses.Contains(nm))
                    nm.ExpenseCollection.Expenses.Remove(nm);

                InsertIntoList(nm.ExpenseCollection.Expenses, nm);

                return await SaveExpenseCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private void InsertIntoList(ObservableCollection<ExpenseModel> list, ExpenseModel model)
        {
            var found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CreateTime < model.CreateTime)
                {
                    list.Insert(i, model);
                    found = true;
                    break;
                }
            }
            if (!found)
                list.Add(model);
        }

        private void InsertIntoList(ObservableCollection<ExpenseCollectionModel> list, ExpenseCollectionModel model)
        {
            var found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Compare(model.Name, list[i].Name, StringComparison.Ordinal) < 0)
                {
                    list.Insert(i, model);
                    found = true;
                    break;
                }
            }
            if (!found)
                list.Add(model);
        }

        public async Task<bool> Delete(ExpenseModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteRequest(_userInformations.Guid, nm.ExpenseCollection.Guid, PossibleActions.Delete,
                    new List<ExpenseModel>() { nm });
                var res = await _dataService.PostExpense(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.Remove : PendingAction.None;

                if (nm.ExpenseCollection.Expenses.Contains(nm))
                    nm.ExpenseCollection.Expenses.Remove(nm);
                else if (nm.ExpenseCollection.DeletedExpenses.Contains(nm))
                    nm.ExpenseCollection.DeletedExpenses.Remove(nm);

                if (nm.PendingAction != PendingAction.None)
                    nm.ExpenseCollection.DeletedExpenses.Add(nm);

                return await SaveExpenseCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public async Task<bool> Save(ExpenseCollectionModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.AddOrUpdate,
                    new List<ExpenseCollectionModel>() { nm });
                var res = await _dataService.PostExpenseCollection(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.AddOrUpdate : PendingAction.None;

                if (!_dataModel.Collections.Contains(nm))
                    _dataModel.Collections.Add(nm);

                return await SaveExpenseCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public async Task<bool> Delete(ExpenseCollectionModel nm)
        {
            try
            {
                var obj = RequestConverter.Instance.ConvertToNoteCollectionRequest(_userInformations.Guid, PossibleActions.Delete,
                    new List<ExpenseCollectionModel>() { nm });
                var res = await _dataService.PostExpenseCollection(obj);
                nm.PendingAction = !res.IsSuccessfull ? PendingAction.Remove : PendingAction.None;

                if (_dataModel.Collections.Contains(nm))
                    _dataModel.Collections.Remove(nm);

                if (nm.PendingAction != PendingAction.None)
                    _dataModel.DeletedCollections.Add(nm);

                return await SaveExpenseCollectionsToStorage() && res.IsSuccessfull;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> SaveUserInformationsToStorage()
        {
            try
            {
                var userInformations = JsonConvert.SerializeObject(_userInformations);
                return await _storageService.SetUserInformations(userInformations);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> RetrieveUserInformationsFromStorage()
        {
            try
            {
                var userInformations = await _storageService.GetUserInformations();
                if (userInformations != null)
                {
                    _userInformations = JsonConvert.DeserializeObject<UserInformationModel>(userInformations);
                    return _userInformations != null;
                }
                else
                {
                    _userInformations = new UserInformationModel(Guid.NewGuid());
                    _userInformations.SaveDataVersion = ActiveDataVersion;
                    return await SaveUserInformationsToStorage();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> SaveExpenseCollectionsToStorage()
        {
            try
            {
                var notesJson = JsonConvert.SerializeObject(_dataModel);
                return await _storageService.SetCachedData(notesJson);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> RetrieveNoteCollectionsFromStorage()
        {
            try
            {
                var cachedNotesJson = await _storageService.GetCachedData();
                if (cachedNotesJson != null)
                {
                    _dataModel = JsonConvert.DeserializeObject<DataModel>(cachedNotesJson);
                    foreach (var collection in _dataModel.Collections)
                    {
                        foreach (var completedNote in collection.Expenses)
                        {
                            completedNote.ExpenseCollection = collection;
                        }
                        foreach (var completedNote in collection.DeletedExpenses)
                        {
                            completedNote.ExpenseCollection = collection;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }
    }

}
