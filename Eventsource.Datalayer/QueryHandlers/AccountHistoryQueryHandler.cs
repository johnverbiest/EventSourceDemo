using System.Data;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using Eventsource.BusinessLogic.Events.FundsTranferCancelled;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Events.FundsWithdrawnCancelled;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.QueryHandlers;

public class AccountHistoryQueryHandler : IQueryHandler<AccountHistoryQuery, AccountHistoryQuery.Result>
{
    private readonly IEventStore _store;

    public AccountHistoryQueryHandler(IEventStore store)
    {
        _store = store;
    }

    public async Task<AccountHistoryQuery.Result> Handle(AccountHistoryQuery query)
    {
        var events = await _store.LoadEvents(
            typeof(AccountCreatedEvent),
            typeof(FundsDepositedEvent),
            typeof(FundStranferCancelledEvent),
            typeof(FundsTransferedEvent),
            typeof(FundsWithdrawnEvent),
            typeof(FundsWithdrawalCancelledEvent),
            typeof(WelcomeMailSentEvent));

        var output = new List<AccountHistoryQuery.Result.HistoryObject>();

        decimal balance = 0;
        foreach (var @event in events)
        {
            switch (@event)
            {
                case AccountCreatedEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = "Account Created"
                        });
                    }
                    break;
                case FundsDepositedEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        balance += e.Amount;
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = $"Funds Deposited: {e.Amount}"
                        });
                    }
                    break;
                case FundStranferCancelledEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = $"Funds Transfer of {e.Amount} to account {e.DestinationAccountNumber} Failed: {e.Reason}"
                        });
                    }
                    break;
                case FundsTransferedEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        balance -= e.Amount;
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = $"Outgoing Funds Transfer of {e.Amount} to account {e.DestinationAccountNumber}"
                        });
                    }
                    if (e.DestinationAccountNumber == query.AccountNumber)
                    {
                        balance += e.Amount;
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = $"Incoming Funds Transfer of {e.Amount} from account {e.AccountNumber}"
                        });
                    }
                    break;
                case FundsWithdrawnEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        balance -= e.Amount;
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = $"Funds withdrawn: {e.Amount}"
                        });
                    }
                    break;
                case FundsWithdrawalCancelledEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = $"Funds withdrawal of {e.Amount} failed: {e.Reason}"
                        });
                    }
                    break;
                case WelcomeMailSentEvent e:
                    if (e.AccountNumber == query.AccountNumber)
                    {
                        output.Add(new AccountHistoryQuery.Result.HistoryObject()
                        {
                            date = e.EventRaised,
                            Balance = balance,
                            Description = "Welcome mail sent"
                        });
                    }
                    break;
                default:
                    throw new ConstraintException($"Event loaded without handler: {@event.GetType().Name}");
            }
        }

        return new AccountHistoryQuery.Result() { Events = output.ToArray() };
    }
}