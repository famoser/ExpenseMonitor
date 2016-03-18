﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Business.Models;

namespace Famoser.ExpenseMonitor.Business.Repositories.Interfaces
{
    public interface INoteRepository
    {
        Task<ObservableCollection<NoteCollectionModel>> GetCollections();
        ObservableCollection<NoteCollectionModel> GetExampleCollections();
        Task<bool> SyncNotes();

        Task<bool> Save(NoteModel nm);
        Task<bool> Delete(NoteModel nm);

        Task<bool> Save(NoteCollectionModel nm);
        Task<bool> Delete(NoteCollectionModel nm);
    }
}