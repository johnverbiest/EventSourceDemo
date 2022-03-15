using JohnVerbiest.CQRS.Commands;

namespace Eventsource.BusinessLogic.Commands.SendNewAccountToManager
{
    public class SendNewAccountToManagerCommand: ICommand
    {
        public int AccountNumber { get; set; }
    }
}