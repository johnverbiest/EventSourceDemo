using JohnVerbiest.CQRS.Commands;

namespace Eventsource.BusinessLogic.Commands.CreateAccount
{
    public record CreateAccountCommand: ICommand
    {
        public string Name { get; set; }
    }
}