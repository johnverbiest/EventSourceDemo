using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.NewAccountMailedToManager;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using Eventsource.BusinessLogic.EventStore.DataAccess;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Commands.SendNewAccountToManager
{
    public class SendNewAccountToManagerCommandHandler: ICommandHandler<SendNewAccountToManagerCommand>
    {
        private readonly IMailer _mailer;
        private readonly IEventDistributor _eventDistributor;
        private readonly IEventPersistance _store;

        public SendNewAccountToManagerCommandHandler(IMailer mailer, IEventDistributor eventDistributor, IEventPersistance store)
        {
            _mailer = mailer;
            _eventDistributor = eventDistributor;
            _store = store;
        }

        public async Task ExecuteAsync(SendNewAccountToManagerCommand command)
        {
            var body = $"Account {command.AccountNumber} with name {await _store.GetAccountName(command.AccountNumber)} was created";
            await Task.WhenAll(
                _mailer.SendEmail(body),
                _eventDistributor.Distribute(new NewAccountMailedToManagerEvent() { AccountNumber = command.AccountNumber, Content = body })
            );
        }
    }
}