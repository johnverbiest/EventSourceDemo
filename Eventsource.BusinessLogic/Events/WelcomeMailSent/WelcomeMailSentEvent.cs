namespace Eventsource.BusinessLogic.Events.WelcomeMailSent
{
    public class WelcomeMailSentEvent: AbstractBusinessLogicEvent
    {
        public int AccountNumber { get; set; }
        public string Content { get; set; }
    }
}