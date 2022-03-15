using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.Tests.EventStore;
using Eventsource.Datalayer.EventHandler;
using FakeItEasy;
using Xunit;

namespace Eventsource.Datalayer.Tests.EventHandler;

public class SaveItAllPolicyTests
{
    [Theory, UnitTest]
    public async Task SaveItAllPolicy_WhenActivated_ShouldSaveTheEvent([Frozen] IEventStore store, TestEvent @event, SaveItAllPolicy sut)
    {
        // Arrange

        // Act
        await sut.Handle(@event);

        // Assert
        A.CallTo(() => store.SaveEvent<IBusinessLogicEvent>(@event)).MustHaveHappenedOnceExactly();
    }
}