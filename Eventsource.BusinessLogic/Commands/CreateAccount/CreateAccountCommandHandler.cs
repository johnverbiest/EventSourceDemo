using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.EventStore.DataAccess;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Commands.CreateAccount
{
    public class CreateAccountCommandHandler: ICommandHandler<CreateAccountCommand>
    {
        private readonly IEventDistributor _eventDistributor;
        private readonly IEventPersistance _eventPersistance;

        public CreateAccountCommandHandler(IEventDistributor eventDistributor, IEventPersistance eventPersistance)
        {
            _eventDistributor = eventDistributor;
            _eventPersistance = eventPersistance;
        }

        public async Task ExecuteAsync(CreateAccountCommand command)
        {
            var highestNumber = await _eventPersistance.GetHighestAccountNumber();

            await _eventDistributor.Distribute(new AccountCreatedEvent()
            {
                AccountNumber = highestNumber + 1,
                Name = command.Name
            });
        }
    }
}