using System.Threading.Tasks;
using Eventsource.BusinessLogic.Commands.SendNewAccountToManager;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Events.AccountCreated.Policies
{
    public class InformManagerPolicy: IEventHandler<AccountCreatedEvent>
    {
        private readonly ICommandQueue _commandQueue;

        public InformManagerPolicy(ICommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
        }

        public Task Handle(AccountCreatedEvent @event)
        {
            return _commandQueue.QueueForExecution(new SendNewAccountToManagerCommand() { AccountNumber = @event.AccountNumber});
        }
    }
}