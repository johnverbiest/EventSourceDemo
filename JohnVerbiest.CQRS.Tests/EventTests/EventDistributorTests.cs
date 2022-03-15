using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using JohnVerbiest.CQRS.Dependencies;
using JohnVerbiest.CQRS.Events;
using Xunit;

namespace JohnVerbiest.CQRS.Tests.EventTests;

public class EventDistributorTests
{
    [Theory, UnitTest]
    public async Task EventDistribbutor_OnDistributing_ShouldRequestHandlers([Frozen] IHandlerRequestDependency dependency, FakeEvent @event, EventDistributor sut)
    {
        // Arrange

        // Act
        await sut.Distribute(@event);

        // Assert
        A.CallTo(() => dependency.GetHandlers<IEventHandler<FakeEvent>>(typeof(IEventHandler<FakeEvent>))).MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task EventDistribbutor_OnDistributing_ShouldRunHandlers([Frozen] IHandlerRequestDependency dependency, IEventHandler<FakeEvent> handler1, IEventHandler<FakeEvent> handler2, FakeEvent @event, EventDistributor sut)
    {
        // Arrange
        A.CallTo(() => dependency.GetHandlers<IEventHandler<FakeEvent>>(typeof(IEventHandler<FakeEvent>))).Returns(new[] { handler1, handler2 });

        // Act
        await sut.Distribute(@event);

        // Assert
        A.CallTo(() => handler1.Handle(@event)).MustHaveHappenedOnceExactly();
        A.CallTo(() => handler2.Handle(@event)).MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task EventDistribbutor_OnDistributing_ShouldRequestCustomInterfaceHandlers([Frozen] IHandlerRequestDependency dependency, FakeEvent @event, EventDistributor sut)
    {
        // Arrange

        // Act
        await sut.Distribute(@event);

        // Assert
        A.CallTo(() => dependency.GetHandlers<IEventHandler<FakeEvent>>(typeof(IEventHandler<IAmCustomInterface>))).MustHaveHappenedOnceExactly();
    }


    [Theory, UnitTest]
    public async Task EventDistribbutor_OnDistributing_ShouldRunCustomInterfaceHandlers([Frozen] IHandlerRequestDependency dependency, IEventHandler<IAmCustomInterface> handler1, IEventHandler<IAmCustomInterface> handler2, FakeEvent @event, EventDistributor sut)
    {
        // Arrange
        A.CallTo(() => dependency.GetHandlers<IEventHandler<FakeEvent>>(typeof(IEventHandler<IAmCustomInterface>))).Returns(new [] {handler1, handler2});

        // Act
        await sut.Distribute(@event);

        // Assert
        A.CallTo(() => handler1.Handle(@event)).MustHaveHappenedOnceExactly();
        A.CallTo(() => handler2.Handle(@event)).MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task EventDistribbutor_OnDistributing_ShouldRequestMainInterfaceHandlers([Frozen] IHandlerRequestDependency dependency, FakeEvent @event, EventDistributor sut)
    {
        // Arrange

        // Act
        await sut.Distribute(@event);

        // Assert
        A.CallTo(() => dependency.GetHandlers<IEventHandler<FakeEvent>>(typeof(IEventHandler<IEvent>))).MustHaveHappenedOnceExactly();
    }


    [Theory, UnitTest]
    public async Task EventDistribbutor_OnDistributing_ShouldRunMainInterfaceHandlers([Frozen] IHandlerRequestDependency dependency, IEventHandler<IEvent> handler1, IEventHandler<IEvent> handler2, FakeEvent @event, EventDistributor sut)
    {
        // Arrange
        A.CallTo(() => dependency.GetHandlers<IEventHandler<FakeEvent>>(typeof(IEventHandler<IEvent>))).Returns(new[] { handler1, handler2 });

        // Act
        await sut.Distribute(@event);

        // Assert
        A.CallTo(() => handler1.Handle(@event)).MustHaveHappenedOnceExactly();
        A.CallTo(() => handler2.Handle(@event)).MustHaveHappenedOnceExactly();
    }
}