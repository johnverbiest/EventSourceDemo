using System;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Events
{
    public interface IBusinessLogicEvent: IEvent
    {
        Guid EventId { get;  }
        DateTime EventRaised { get; }
        int AccountNumber { get; }
    }
}