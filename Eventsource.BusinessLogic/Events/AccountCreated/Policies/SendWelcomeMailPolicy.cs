using System;
using System.Threading.Tasks;
using Eventsource.BusinessLogic.Commands.SendWelcomeMail;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Events.AccountCreated.Policies
{
    public class SendWelcomeMailPolicy: IEventHandler<AccountCreatedEvent>
    {
        private readonly ICommandQueue _commandQueue;

        public SendWelcomeMailPolicy(ICommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
        }

        public Task Handle(AccountCreatedEvent @event)
        {
            return _commandQueue.QueueForExecution(new SendWelcomeMailCommand() { AccountNumber = @event.AccountNumber });
        }
    }
}