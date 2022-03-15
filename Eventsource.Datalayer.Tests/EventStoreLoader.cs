using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using FakeItEasy;

namespace Eventsource.Datalayer.Tests;

public static class EventStoreLoader
{
    public static void InjectEvents(this IEventStore store, params IBusinessLogicEvent[] events)
    {
        A.CallTo(() => store.LoadEvents()).WithAnyArguments().Returns(events);
    }
}