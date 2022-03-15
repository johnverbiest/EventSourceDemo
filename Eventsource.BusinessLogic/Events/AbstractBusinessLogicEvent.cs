using System;

namespace Eventsource.BusinessLogic.Events
{
    public class AbstractBusinessLogicEvent: IBusinessLogicEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime EventRaised { get; set; } = DateTime.UtcNow;
    }
}