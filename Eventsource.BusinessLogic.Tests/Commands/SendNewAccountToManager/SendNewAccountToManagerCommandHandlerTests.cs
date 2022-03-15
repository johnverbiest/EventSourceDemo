using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.SendNewAccountToManager;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.NewAccountMailedToManager;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Commands.SendNewAccountToManager;

public class SendNewAccountToManagerCommandHandlerTests
{
    [Theory, UnitTest]
    public async Task SendNewAccountToManagerCommandHandler_OnHandle_ShouldSendEmail([Frozen] IMailer mailer, SendNewAccountToManagerCommand command, SendNewAccountToManagerCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => mailer.SendEmail(A<string>.That.Matches(x => x.Contains(command.AccountNumber.ToString()))))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task SendNewAccountToManagerCommandHandler_OnHandle_ShouldEmailAccountnumberToManager([Frozen] IEventDistributor distributor, SendNewAccountToManagerCommand command, SendNewAccountToManagerCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<NewAccountMailedToManagerEvent>.That.Matches(e =>
                e.AccountNumber == command.AccountNumber && e.Content.Contains(command.AccountNumber.ToString()))))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task SendNewAccountToManagerCommandHandler_OnHandle_ShouldEmailNameToManager([Frozen] IEventDistributor distributor, [Frozen]IEventPersistance store, AccountCreatedEvent initialEvent, SendNewAccountToManagerCommand command, SendNewAccountToManagerCommandHandler sut)
    {
        // Arrange
        command.AccountNumber = initialEvent.AccountNumber;
        A.CallTo(() => store.LoadEvents()).WithAnyArguments().Returns(new[] { initialEvent });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<NewAccountMailedToManagerEvent>.That.Matches(e =>
                e.AccountNumber == command.AccountNumber && e.Content.Contains(initialEvent.Name))))
            .MustHaveHappenedOnceExactly();
    }
}