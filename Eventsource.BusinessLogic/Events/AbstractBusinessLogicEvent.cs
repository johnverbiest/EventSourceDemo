using System;

namespace Eventsource.BusinessLogic.Events
{
    public abstract class AbstractBusinessLogicEvent: IBusinessLogicEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime EventRaised { get; set; } = DateTime.UtcNow;
        public abstract int AccountNumber { get; set; }
    }
}