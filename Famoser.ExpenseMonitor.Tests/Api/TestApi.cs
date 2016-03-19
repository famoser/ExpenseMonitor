using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Business.Converters;
using Famoser.ExpenseMonitor.Business.Models;
using Famoser.ExpenseMonitor.Data.Enum;
using Famoser.ExpenseMonitor.Data.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.ExpenseMonitor.Tests.Api
{
    [TestClass]
    public class TestApi
    {
        [TestMethod]
        public async Task TestNotes()
        {
            var note1 = new ExpenseModel()
            {
                Amount = 2.5,
                Description = "hallo welt",
                CreateTime = DateTime.Now,
                Guid = Guid.NewGuid()
            };
            var note2 = new ExpenseModel()
            {
                Amount = 3.1,
                Description = "hallo welt 1",
                CreateTime = DateTime.Today,
                Guid = Guid.NewGuid()
            };
            var note3 = new ExpenseModel()
            {
                Amount = 34,
                Description = "hallo welt 2",
                CreateTime = DateTime.Today - TimeSpan.FromDays(1),
                Guid = Guid.NewGuid()
            };
            var ds = new DataService();

            /*

            var addRequest = RequestConverter.Instance.ConvertToNoteRequest(ApiTestHelper.TestUserGuid,
                PossibleActions.AddOrUpdate, new List<ExpenseModel>() { note1, note2, note3 });
            

            //act
            //check if 0 Expenses
            var notes = await ds.GetExpense(ApiTestHelper.TestUserGuid);
            ApiAssertHelper.CheckBaseResponse(notes);
            Assert.IsTrue(notes.Expenses == null || !notes.Expenses.Any());

            //add Expenses;
            var res = await ds.PostExpense(addRequest);
            ApiAssertHelper.CheckBooleanResponse(res);

            //check if 2 Expenses
            notes = await ds.GetExpense(ApiTestHelper.TestUserGuid);
            ApiAssertHelper.CheckBaseResponse(notes);
            Assert.IsTrue(notes.Expenses != null && notes.Expenses.Count == 3);

            //remove 1 Expenses;
            res = await ds.PostExpense(remove1);
            ApiAssertHelper.CheckBooleanResponse(res);

            //check if 2 Note, check Date
            notes = await ds.GetExpense(ApiTestHelper.TestUserGuid);
            ApiAssertHelper.CheckBaseResponse(notes);
            Assert.IsTrue(notes.Expenses != null && notes.Expenses.Count == 2);
            Assert.IsTrue(notes.Expenses[0].Guid == note2.Guid);
            Assert.IsTrue(notes.Expenses[0].CreateTime - note2.CreateTime < TimeSpan.FromSeconds(1));
            Assert.IsTrue(notes.Expenses[1].Guid == note3.Guid);
            Assert.IsTrue(notes.Expenses[1].CreateTime - note3.CreateTime < TimeSpan.FromSeconds(1));

            //remove 1 Note left;
            res = await ds.PostExpense(remove2);
            ApiAssertHelper.CheckBooleanResponse(res);

            //check if 0 Expenses
            notes = await ds.GetExpense(ApiTestHelper.TestUserGuid);
            ApiAssertHelper.CheckBaseResponse(notes);
            Assert.IsTrue(notes.Expenses == null || !notes.Expenses.Any());
            */
        }
    }
}
