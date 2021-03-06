using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer.Appliers;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.QueryHandlers
{
    public class HighestAccountNumberQueryHandler: IQueryHandler<HighestAccountNumberQuery, HighestAccountNumberQuery.Result>
    {
        private readonly IEventStore _store;

        public HighestAccountNumberQueryHandler(IEventStore store)
        {
            _store = store;
        }

        public async Task<HighestAccountNumberQuery.Result> Handle(HighestAccountNumberQuery query)
        {
            var events = (await _store.LoadEvents(new int[] {}, typeof(AccountCreatedEvent))).OfType<AccountCreatedEvent>();
            var results = events.Any() ? events.Max(x => x.Apply().accountNumber) : 0;
            return new HighestAccountNumberQuery.Result() { HighestAccountNumber = results };
        }
    }
}