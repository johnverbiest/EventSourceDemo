using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Dependencies;
using Xunit;

namespace JohnVerbiest.CQRS.Tests.CommandsTests;

public class CommandQueueTests
{
    [Theory, UnitTest]
    public async Task CommandQueue_OnQueueing_ShouldGetCorrectHandler([Frozen] IHandlerRequestDependency _dependency, FakeCommand command, CommandQueue sut)
    {
        // Arrange

        // Act
        await sut.QueueForExecution(command);

        // Assert
        A.CallTo(() => _dependency.GetHandler<ICommandHandler<FakeCommand>>()).MustHaveHappenedOnceExactly();
    }

}