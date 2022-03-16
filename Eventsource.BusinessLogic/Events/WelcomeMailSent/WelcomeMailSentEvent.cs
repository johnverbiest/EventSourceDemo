namespace Eventsource.BusinessLogic.Events.WelcomeMailSent
{
    public class WelcomeMailSentEvent: AbstractBusinessLogicEvent
    {
        public override int AccountNumber { get; set; }
        public string Content { get; set; }
    }
}