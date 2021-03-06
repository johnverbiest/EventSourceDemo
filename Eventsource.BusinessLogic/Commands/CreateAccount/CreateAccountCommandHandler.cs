using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand>
    {
        private readonly IEventDistributor _eventDistributor;
        private readonly IQueryHandler<HighestAccountNumberQuery, HighestAccountNumberQuery.Result> _highestAccountNumberQueryHandler;

        public CreateAccountCommandHandler(IEventDistributor eventDistributor, IQueryHandler<HighestAccountNumberQuery, HighestAccountNumberQuery.Result> highestAccountNumberQueryHandler)
        {
            _eventDistributor = eventDistributor;
            _highestAccountNumberQueryHandler = highestAccountNumberQueryHandler;
        }


        public static object theLock = new object();
        public Task ExecuteAsync(CreateAccountCommand command)
        {
            lock (theLock)
            {
                var highestNumber = _highestAccountNumberQueryHandler.Handle(new HighestAccountNumberQuery()).Result;

                _eventDistributor.Distribute(new AccountCreatedEvent()
                {
                    AccountNumber = highestNumber.HighestAccountNumber + 1,
                    Name = command.Name
                }).Wait();
            }
            return Task.CompletedTask;
        }
    }
}