using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.NewAccountMailedToManager;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Commands.SendNewAccountToManager
{
    public class SendNewAccountToManagerCommandHandler: ICommandHandler<SendNewAccountToManagerCommand>
    {
        private readonly IMailer _mailer;
        private readonly IEventDistributor _eventDistributor;
        private readonly IQueryHandler<AccountNameQuery, AccountNameQuery.Result> _accountNameQueryHandler;

        public SendNewAccountToManagerCommandHandler(IMailer mailer, IEventDistributor eventDistributor, IQueryHandler<AccountNameQuery, AccountNameQuery.Result> accountNameQueryHandler)
        {
            _mailer = mailer;
            _eventDistributor = eventDistributor;
            _accountNameQueryHandler = accountNameQueryHandler;
        }

        public async Task ExecuteAsync(SendNewAccountToManagerCommand command)
        {
            var body = $"Account {command.AccountNumber} with name {(await _accountNameQueryHandler.Handle(new AccountNameQuery() {AccountNumber = command.AccountNumber})).AccountName} was created";
            await Task.WhenAll(
                _mailer.SendEmail(body),
                _eventDistributor.Distribute(new NewAccountMailedToManagerEvent() { AccountNumber = command.AccountNumber, Content = body })
            );
        }
    }
}