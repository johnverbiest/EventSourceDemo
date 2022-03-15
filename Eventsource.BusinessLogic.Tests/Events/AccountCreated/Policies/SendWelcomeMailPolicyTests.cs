using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.SendWelcomeMail;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.AccountCreated.Policies;
using FakeItEasy;
using JohnVerbiest.CQRS.Commands;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Events.AccountCreated.Policies;

public class SendWelcomeMailPolicyTests
{
    [Theory, UnitTest]
    public async Task SendWelcomeMailPolicy_WhenANewAccountIsCreated_ShouldQueueSendMailCommand([Frozen] ICommandQueue commandQueue, AccountCreatedEvent @event, SendWelcomeMailPolicy sut) 
    {
        // Arrange

        // Act
        await sut.Handle(@event);

        // Assert
        A.CallTo(() =>
                commandQueue.QueueForExecution(
                    A<SendWelcomeMailCommand>.That.Matches(c => c.AccountNumber == @event.AccountNumber)))
            .MustHaveHappenedOnceExactly();
    }
}