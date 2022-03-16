namespace Eventsource.BusinessLogic.Events.FundsDeposited
{
    public class FundsDepositedEvent : AbstractBusinessLogicEvent

    {
        public override int AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}