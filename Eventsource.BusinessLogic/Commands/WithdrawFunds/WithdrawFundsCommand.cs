using JohnVerbiest.CQRS.Commands;

namespace Eventsource.BusinessLogic.Commands.WithdrawFunds
{
    public class WithdrawFundsCommand: ICommand
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}