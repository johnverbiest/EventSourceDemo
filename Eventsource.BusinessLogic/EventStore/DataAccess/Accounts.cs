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
            var events = (await store.LoadEvents(typeof(AccountCreatedEvent))).OfType<AccountCreatedEvent>();
            return events.Any() ? events.Max(x => x.AccountNumber) : 0;
        }

        public static async Task<string> GetAccountName(this IEventPersistance store, int accountNumber)
        {
            var events = (await store.LoadEvents(typeof(AccountCreatedEvent)));
            var currentName = "<Unknown>";
            foreach (var @event in events)
            {
                switch (@event)
                {
                    case AccountCreatedEvent e:
                        if (e.AccountNumber == accountNumber) currentName = e.Name;
                        break;
                }
            }

            return currentName;
        }
    }
}