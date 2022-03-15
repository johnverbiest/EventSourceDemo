using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.QueryHandlers
{
    public class HighestAccountNumberQueryHandler: IQueryHandler<HighestAccountNumberQuery, HighestAccountNumberQuery.Result>
    {
        private readonly IEventPersistance _store;

        public HighestAccountNumberQueryHandler(IEventPersistance store)
        {
            _store = store;
        }

        public async Task<HighestAccountNumberQuery.Result> Handle(HighestAccountNumberQuery query)
        {
            var events = (await _store.LoadEvents(typeof(AccountCreatedEvent))).OfType<AccountCreatedEvent>();
            var results = events.Any() ? events.Max(x => x.AccountNumber) : 0;
            return new HighestAccountNumberQuery.Result() { HighestAccountNumber = results };
        }
    }
}