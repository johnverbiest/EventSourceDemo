namespace Eventsource.BusinessLogic.Events.FundsTransferedIn
{
    public class FundsTransferedInEvent: AbstractBusinessLogicEvent
    {
        public override int AccountNumber { get; set; }
        public int OriginAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}