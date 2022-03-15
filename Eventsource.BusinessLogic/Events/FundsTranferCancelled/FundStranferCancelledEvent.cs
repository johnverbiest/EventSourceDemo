namespace Eventsource.BusinessLogic.Events.FundsTranferCancelled
{
    public class FundStranferCancelledEvent: AbstractBusinessLogicEvent
    {
        public int AccountNumber { get; set; }
        public int DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
}