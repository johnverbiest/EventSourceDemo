namespace Eventsource.BusinessLogic.Events.FundsTransfered
{
    public class FundsTransferedEvent: AbstractBusinessLogicEvent
    {
        public int AccountNumber { get; set; }
        public int DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}