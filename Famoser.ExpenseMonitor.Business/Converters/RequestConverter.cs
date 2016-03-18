using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Data.Entities;
using Famoser.ExpenseMonitor.Data.Entities.Communication;
using Famoser.ExpenseMonitor.Data.Enum;
using Famoser.FrameworkEssentials.Singleton;

namespace Famoser.ExpenseMonitor.Business.Converters
{
    public class RequestConverter : SingletonBase<RequestConverter>
    {
        public NoteRequest ConvertToNoteRequest(Guid userGuid, Guid collectionGuid, PossibleActions action, IEnumerable<NoteModel> notes)
        {
            return new NoteRequest(action,userGuid)
            {
                Notes = ConvertAllToNoteEntity(notes),
                NoteCollectionGuid = collectionGuid
            };
        }

        private List<NoteEntity> ConvertAllToNoteEntity(IEnumerable<NoteModel> notes)
        {
            return notes.Select(ConvertToNoteEntity).ToList();
        }

        private NoteEntity ConvertToNoteEntity(NoteModel noteModel)
        {
            return new NoteEntity()
            {
                 Guid = noteModel.Guid,
                 Content = noteModel.Content,
                 CreateTime = noteModel.CreateTime,
                 IsCompletedBool = noteModel.IsCompleted
            };
        }

        public NoteCollectionRequest ConvertToNoteCollectionRequest(Guid userGuid, PossibleActions action, IEnumerable<NoteCollectionModel> collections)
        {
            return new NoteCollectionRequest(action, userGuid)
            {
                NoteCollections = ConvertAllToNoteCollectionEntity(collections)
            };
        }

        private List<NoteCollectionEntity> ConvertAllToNoteCollectionEntity(IEnumerable<NoteCollectionModel> collections)
        {
            return collections.Select(ConvertToNoteCollectionEntity).ToList();
        }

        private NoteCollectionEntity ConvertToNoteCollectionEntity(NoteCollectionModel collection)
        {
            return new NoteCollectionEntity()
            {
                Guid = collection.Guid,
                Name = collection.Name,
                CreateTime = collection.CreateTime
            };
        }
    }
}
