namespace Eventsource.BusinessLogic.Events.FundsTransfered
{
    public class FundsTransferedOutEvent: AbstractBusinessLogicEvent
    {
        public override int AccountNumber { get; set; }
        public int DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}