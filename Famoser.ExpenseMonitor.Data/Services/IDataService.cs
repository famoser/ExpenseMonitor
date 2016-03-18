using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Data.Entities.Communication;

namespace Famoser.ExpenseMonitor.Data.Services
{
    public interface IDataService
    {
        Task<BooleanResponse> PostNote(NoteRequest request);
        Task<BooleanResponse> PostNoteCollection(NoteCollectionRequest request);
        Task<NoteResponse> GetNotes(NoteRequest request);
        Task<NoteCollectionResponse> GetNoteCollections(NoteCollectionRequest request);
    }
}
