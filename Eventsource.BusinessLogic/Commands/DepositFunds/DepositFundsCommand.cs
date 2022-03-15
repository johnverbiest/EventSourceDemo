using JohnVerbiest.CQRS.Commands;

namespace Eventsource.BusinessLogic.Commands.DepositFunds
{
    public class DepositFundsCommand: ICommand
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}