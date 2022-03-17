using System.Data;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsTransferedIn;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer.Appliers;
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
        var events = await _store.LoadEvents(new [] {query.AccountNumber}, typeof(FundsDepositedEvent), typeof(FundsWithdrawnEvent), typeof(FundsTransferedOutEvent), typeof(FundsTransferedInEvent));
        decimal balance = 0;

        foreach (var @event in events)
        {
            switch (@event)
            {
                case FundsDepositedEvent e:
                    if (e.AccountNumber == query.AccountNumber) balance = e.Apply(balance);
                    break;
                case FundsWithdrawnEvent e:
                    if (e.AccountNumber == query.AccountNumber) balance = e.Apply(balance);
                    break;
                case FundsTransferedOutEvent e:
                    if (e.AccountNumber == query.AccountNumber) balance = e.Apply(balance);
                    break;
                case FundsTransferedInEvent e:
                    if (e.AccountNumber == query.AccountNumber) balance = e.Apply(balance);
                    break;
                default:
                    throw new ConstraintException($"Event loaded without handler: {@event.GetType().Name}");
            }
        }

        return new AccountBalanceQuery.Result() { Balance = balance };
    }
}