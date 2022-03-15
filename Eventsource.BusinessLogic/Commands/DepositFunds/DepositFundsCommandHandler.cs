using System.Threading.Tasks;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Commands.DepositFunds
{
    public class DepositFundsCommandHandler: ICommandHandler<DepositFundsCommand>
    {
        private readonly IEventDistributor _eventDistributor;

        public DepositFundsCommandHandler(IEventDistributor eventDistributor)
        {
            _eventDistributor = eventDistributor;
        }

        public Task ExecuteAsync(DepositFundsCommand command)
        {
            return _eventDistributor.Distribute(new FundsDepositedEvent()
                { AccountNumber = command.AccountNumber, Amount = command.Amount });
        }
    }
}