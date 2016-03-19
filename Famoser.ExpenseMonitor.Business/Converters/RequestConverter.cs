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
        public ExpenseRequest ConvertToNoteRequest(Guid userGuid, Guid collectionGuid, PossibleActions action, IEnumerable<ExpenseModel> notes)
        {
            return new ExpenseRequest(action,userGuid)
            {
                Expenses = ConvertAllToNoteEntity(notes),
                ExpenseCollectionGuid = collectionGuid
            };
        }

        private List<ExpenseEntity> ConvertAllToNoteEntity(IEnumerable<ExpenseModel> notes)
        {
            return notes.Select(ConvertToNoteEntity).ToList();
        }

        private ExpenseEntity ConvertToNoteEntity(ExpenseModel expenseModel)
        {
            return new ExpenseEntity()
            {
                 Guid = expenseModel.Guid,
                 Description = expenseModel.Description,
                 CreateTime = expenseModel.CreateTime,
                 Amount = expenseModel.Amount
            };
        }

        public ExpenseCollectionRequest ConvertToNoteCollectionRequest(Guid userGuid, PossibleActions action, IEnumerable<ExpenseCollectionModel> collections)
        {
            return new ExpenseCollectionRequest(action, userGuid)
            {
                ExpenseCollections = ConvertAllToNoteCollectionEntity(collections)
            };
        }

        private List<ExpenseCollectionEntity> ConvertAllToNoteCollectionEntity(IEnumerable<ExpenseCollectionModel> collections)
        {
            return collections.Select(ConvertToNoteCollectionEntity).ToList();
        }

        private ExpenseCollectionEntity ConvertToNoteCollectionEntity(ExpenseCollectionModel collection)
        {
            return new ExpenseCollectionEntity()
            {
                Guid = collection.Guid,
                Name = collection.Name,
                CreateTime = collection.CreateTime
            };
        }
    }
}
