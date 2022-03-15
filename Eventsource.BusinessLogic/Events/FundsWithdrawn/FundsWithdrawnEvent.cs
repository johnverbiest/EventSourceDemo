namespace Eventsource.BusinessLogic.Events.FundsWithdrawn
{
    public class FundsWithdrawnEvent: AbstractBusinessLogicEvent
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}