using System.Data;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.QueryHandlers;

public class AccountBalanceQueryHandler: IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result>
{
    private readonly IEventStore _store;

    public AccountBalanceQueryHandler(IEventStore store)
    {
        _store = store;
    }

    public async Task<AccountBalanceQuery.Result> Handle(AccountBalanceQuery query)
    {
        var events = await _store.LoadEvents(typeof(FundsDepositedEvent), typeof(FundsWithdrawnEvent));
        decimal balance = 0;

        foreach (var @event in events)
        {
            switch (@event)
            {
                case FundsDepositedEvent e:
                    if (e.AccountNumber == query.AccountNumber) balance += e.Amount;
                    break;
                case FundsWithdrawnEvent e:
                    if (e.AccountNumber == query.AccountNumber) balance -= e.Amount;
                    break;
                default:
                    throw new ConstraintException($"Event loaded without handler: {@event.GetType().Name}");
            }
        }

        return new AccountBalanceQuery.Result() { Balance = balance };
    }
}