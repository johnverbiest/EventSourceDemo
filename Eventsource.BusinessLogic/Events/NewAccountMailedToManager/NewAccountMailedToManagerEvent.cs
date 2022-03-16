namespace Eventsource.BusinessLogic.Events.NewAccountMailedToManager
{
    public class NewAccountMailedToManagerEvent: AbstractBusinessLogicEvent
    {
        public override int AccountNumber { get; set; }
        public string Content { get; set; }
    }
}