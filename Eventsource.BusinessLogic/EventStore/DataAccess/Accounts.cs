using System.Linq;
using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;

namespace Eventsource.BusinessLogic.EventStore.DataAccess
{
    public static class Accounts
    {
        public static async Task<int> GetHighestAccountNumber(this IEventPersistance store)
        {
            var test = (await store.LoadEvents(typeof(AccountCreatedEvent))).OfType<AccountCreatedEvent>();
            return test.Any() ? test.Max(x => x.AccountNumber) : 0;

        }
    }
}