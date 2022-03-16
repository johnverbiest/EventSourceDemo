namespace Eventsource.BusinessLogic.Events.FundsWithdrawn
{
    public class FundsWithdrawnEvent: AbstractBusinessLogicEvent
    {
        public override int AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}