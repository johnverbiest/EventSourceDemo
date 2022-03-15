using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.EventStore;
using FakeItEasy;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.EventStore;

public class SaveItAllPolicyTests
{
    [Theory, UnitTest]
    public async Task SaveItAllPolicy_WhenActivated_ShouldSaveTheEvent([Frozen] IEventPersistance store, TestEvent @event, SaveItAllPolicy sut)
    {
        // Arrange

        // Act
        await sut.Handle(@event);

        // Assert
        A.CallTo(() => store.SaveEvent<IBusinessLogicEvent>(@event)).MustHaveHappenedOnceExactly();
    }
}