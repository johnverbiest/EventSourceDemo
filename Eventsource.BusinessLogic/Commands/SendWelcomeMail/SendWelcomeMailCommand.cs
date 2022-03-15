using JohnVerbiest.CQRS.Commands;

namespace Eventsource.BusinessLogic.Commands.SendWelcomeMail
{
    public class SendWelcomeMailCommand: ICommand
    {
        public int AccountNumber { get; set; }
    }
}