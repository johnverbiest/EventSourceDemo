namespace Eventsource.BusinessLogic.Events.FundsWithdrawnCancelled
{
    public class FundsWithdrawalCancelledEvent: AbstractBusinessLogicEvent
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
}