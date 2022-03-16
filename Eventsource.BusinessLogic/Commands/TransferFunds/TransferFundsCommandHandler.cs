using System.Threading.Tasks;
using Eventsource.BusinessLogic.Events.FundsTranferCancelled;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsTransferedIn;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Commands.TransferFunds
{
    public class TransferFundsCommandHandler: ICommandHandler<TransferFundsCommand>
    {
        private readonly IEventDistributor _eventDistributor;
        private readonly IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> _balanceQueryHandler;

        public TransferFundsCommandHandler(IEventDistributor eventDistributor, IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> balanceQueryHandler)
        {
            _eventDistributor = eventDistributor;
            _balanceQueryHandler = balanceQueryHandler;
        }

        public async Task ExecuteAsync(TransferFundsCommand command)
        {
            var balance = (await _balanceQueryHandler.Handle(new AccountBalanceQuery()
                { AccountNumber = command.AccountNumber })).Balance;

            if (balance >= command.Amount)
            {
                await _eventDistributor.Distribute(new FundsTransferedOutEvent()
                {
                    AccountNumber = command.AccountNumber, 
                    DestinationAccountNumber = command.DestinationAccountNumber,
                    Amount = command.Amount
                });
                await _eventDistributor.Distribute(new FundsTransferedInEvent()
                {
                    AccountNumber = command.DestinationAccountNumber,
                    OriginAccountNumber = command.AccountNumber,
                    Amount = command.Amount
                });
            }
            else
            {
                await _eventDistributor.Distribute(new FundStranferCancelledEvent()
                {
                    AccountNumber = command.AccountNumber,
                    DestinationAccountNumber = command.DestinationAccountNumber,
                    Amount = command.Amount,
                    Reason = "Insufficient Funds"
                });
            }
        }
    }
}