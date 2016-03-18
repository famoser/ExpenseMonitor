using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Data.Entities;
using Famoser.FrameworkEssentials.Singleton;

namespace Famoser.ExpenseMonitor.Business.Converters
{
    public class ResponseConverter : SingletonBase<ResponseConverter>
    {
        public ExpenseModel Convert(ExpenseEntity expense)
        {
            return new ExpenseModel()
            {
                Guid = expense.Guid,
                Description = expense.Content,
                CreateTime = expense.CreateTime,
                Amount = expense.Amount
            };
        }

        public void WriteValues(ExpenseEntity expense, ExpenseModel model)
        {
            model.Amount = expense.Amount;
            model.Description = expense.Content;
            model.CreateTime = expense.CreateTime;
        }

        public ExpenseCollectionModel Convert(ExpenseCollectionEntity entity)
        {
            return new ExpenseCollectionModel()
            {
                Guid = entity.Guid,
                Name = entity.Name,
                CreateTime = entity.CreateTime
            };
        }

        public void WriteValues(ExpenseCollectionEntity expense, ExpenseCollectionModel model)
        {
            model.Name = expense.Name;
            model.CreateTime = expense.CreateTime;
        }
    }
}
