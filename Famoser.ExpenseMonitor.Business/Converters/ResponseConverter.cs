using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Data.Entities;
using Famoser.FrameworkEssentials.Singleton;

namespace Famoser.ExpenseMonitor.Business.Converters
{
    public class ResponseConverter : SingletonBase<ResponseConverter>
    {
        public NoteModel Convert(NoteEntity note)
        {
            return new NoteModel()
            {
                Guid = note.Guid,
                Content = note.Content,
                CreateTime = note.CreateTime,
                IsCompleted = note.IsCompletedBool
            };
        }

        public void WriteValues(NoteEntity note, NoteModel model)
        {
            model.IsCompleted = note.IsCompletedBool;
            model.Content = note.Content;
            model.CreateTime = note.CreateTime;
        }

        public NoteCollectionModel Convert(NoteCollectionEntity entity)
        {
            return new NoteCollectionModel()
            {
                Guid = entity.Guid,
                Name = entity.Name,
                CreateTime = entity.CreateTime
            };
        }

        public void WriteValues(NoteCollectionEntity note, NoteCollectionModel model)
        {
            model.Name = note.Name;
            model.CreateTime = note.CreateTime;
        }
    }
}
