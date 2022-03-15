using JohnVerbiest.CQRS.Commands;

namespace Eventsource.BusinessLogic.Commands.TransferFunds
{
    public class TransferFundsCommand: ICommand
    {
        public int AccountNumber { get; set; }
        public int DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}