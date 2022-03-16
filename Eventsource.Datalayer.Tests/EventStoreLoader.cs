using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.Events.AccountCreated;
using FakeItEasy;

namespace Eventsource.Datalayer.Tests;

public static class EventStoreLoader
{
    public static void InjectEvents(this IEventStore store, params IBusinessLogicEvent[] events)
    {
        A.CallTo(() => store.LoadEvents(new [] { 1 }, typeof(AccountCreatedEvent))).WithAnyArguments().Returns(events);
    }
}