using Eventsource.BusinessLogic.Events;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.Tests.EventStore;

public class TestEvent: AbstractBusinessLogicEvent
{
    public override int AccountNumber { get; set; }
}