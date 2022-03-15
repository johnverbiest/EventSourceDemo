using System.Threading.Tasks;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Events.FundsWithdrawnCancelled;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Commands.WithdrawFunds
{
    public class WithdrawFundsCommandHandler: ICommandHandler<WithdrawFundsCommand>
    {
        private readonly IEventDistributor _eventDistributor;
        private readonly IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> _balanceQueryHandler;

        public WithdrawFundsCommandHandler(IEventDistributor eventDistributor, IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> balanceQueryHandler)
        {
            _eventDistributor = eventDistributor;
            _balanceQueryHandler = balanceQueryHandler;
        }

        public async Task ExecuteAsync(WithdrawFundsCommand command)
        {
            var currentbalance = (await _balanceQueryHandler.Handle(new AccountBalanceQuery()
                { AccountNumber = command.AccountNumber })).Balance;

            if (currentbalance >= command.Amount)
            {
                await _eventDistributor.Distribute(new FundsWithdrawnEvent()
                    { AccountNumber = command.AccountNumber, Amount = command.Amount });
            }
            else
            {
                await _eventDistributor.Distribute(new FundsWithdrawalCancelledEvent()
                    { AccountNumber = command.AccountNumber, Amount = command.Amount, Reason = "Insufficient Funds" });
            }
        }
    }
}