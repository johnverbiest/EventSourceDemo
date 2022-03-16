using System.Data;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.QueryHandlers;

public class AllActiveAccountsQueryHandler: IQueryHandler<AllActiveAccountsQuery, AllActiveAccountsQuery.Result>
{
    private readonly IEventStore _store;

    public AllActiveAccountsQueryHandler(IEventStore store)
    {
        _store = store;
    }

    public async Task<AllActiveAccountsQuery.Result> Handle(AllActiveAccountsQuery query)
    {
        var events = (await _store.LoadEvents(new int[] {}, typeof(AccountCreatedEvent)));

        // Set default value
        var results = new List<AllActiveAccountsQuery.Account>();

        // Go over all the events
        foreach (var @event in events)
        {
            switch (@event)
            {
                case AccountCreatedEvent e:
                    results.Add(new AllActiveAccountsQuery.Account()
                    {
                        AccountNumber = e.AccountNumber,
                        Name = e.Name
                    });
                    break;
                default:
                    throw new ConstraintException($"Event loaded without handler: {@event.GetType().Name}");
            }
        }

        return new AllActiveAccountsQuery.Result() { Accounts = results.ToArray()};
    }
}