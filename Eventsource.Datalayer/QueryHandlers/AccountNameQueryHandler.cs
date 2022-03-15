using System.Data;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.QueryHandlers;

public class AccountNameQueryHandler: IQueryHandler<AccountNameQuery, AccountNameQuery.Result>
{
    private readonly IEventPersistance _store;

    public AccountNameQueryHandler(IEventPersistance store)
    {
        _store = store;
    }

    public async Task<AccountNameQuery.Result> Handle(AccountNameQuery query)
    {
        var events = (await _store.LoadEvents(typeof(AccountCreatedEvent)));

        // Set default value
        var currentName = "<Unknown>";

        // Go over all the events
        foreach (var @event in events)
        {
            switch (@event)
            {
                case AccountCreatedEvent e:
                    if (e.AccountNumber == query.AccountNumber) currentName = e.Name;
                    break;
                default:
                    throw new ConstraintException($"Event loaded without handler: {@event.GetType().Name}");
            }
        }

        return new AccountNameQuery.Result() { AccountName = currentName };
    }
}