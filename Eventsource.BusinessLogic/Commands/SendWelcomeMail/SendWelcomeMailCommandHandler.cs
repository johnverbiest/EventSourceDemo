using System;
using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Commands.SendWelcomeMail
{
    public class SendWelcomeMailCommandHandler: ICommandHandler<SendWelcomeMailCommand>
    {
        private readonly IMailer _mailer;
        private readonly IEventDistributor _eventDistributor;

        public SendWelcomeMailCommandHandler(IMailer mailer, IEventDistributor eventDistributor)
        {
            _mailer = mailer;
            _eventDistributor = eventDistributor;
        }

        public Task ExecuteAsync(SendWelcomeMailCommand command)
        {
            var body = $"Welcome account {command.AccountNumber}";
            return Task.WhenAll(
                _mailer.SendEmail(body),
                _eventDistributor.Distribute(new WelcomeMailSentEvent() {AccountNumber = command.AccountNumber, Content = body})
                );
        }
    }
}