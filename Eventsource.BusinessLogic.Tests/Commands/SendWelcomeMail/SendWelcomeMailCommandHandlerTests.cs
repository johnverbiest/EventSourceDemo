using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.SendWelcomeMail;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Commands.SendWelcomeMail;

public class SendWelcomeMailCommandHandlerTests
{
    [Theory, UnitTest]
    public async Task SendWelcomeMailCommandHandler_OnHandle_ShouldSendEmail([Frozen] IMailer mailer, SendWelcomeMailCommand command, SendWelcomeMailCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => mailer.SendEmail(A<string>.That.Matches(x => x.Contains(command.AccountNumber.ToString()))))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task SendWelcomeMailCommandHandler_OnHandle_ShouldRaiseWelcomeMailSentEvent([Frozen] IEventDistributor distributor, SendWelcomeMailCommand command, SendWelcomeMailCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<WelcomeMailSentEvent>.That.Matches(e =>
                e.AccountNumber == command.AccountNumber && e.Content.Contains(command.AccountNumber.ToString()))))
            .MustHaveHappenedOnceExactly();
    }
}