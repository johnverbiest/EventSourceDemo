using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.SendNewAccountToManager;
using Eventsource.BusinessLogic.Commands.SendWelcomeMail;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.AccountCreated.Policies;
using FakeItEasy;
using JohnVerbiest.CQRS.Commands;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Events.AccountCreated.Policies;

public class InformManagerPolicyTests
{
    [Theory, UnitTest]
    public async Task SendWelcomeMailPolicy_WhenANewAccountIsCreated_ShouldQueueInformManagerCommand([Frozen] ICommandQueue commandQueue, AccountCreatedEvent @event, InformManagerPolicy sut) 
    {
        // Arrange

        // Act
        await sut.Handle(@event);

        // Assert
        A.CallTo(() =>
                commandQueue.QueueForExecution(
                    A<SendNewAccountToManagerCommand>.That.Matches(c => c.AccountNumber == @event.AccountNumber)))
            .MustHaveHappenedOnceExactly();
    }
}