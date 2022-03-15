using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.SendNewAccountToManager;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.NewAccountMailedToManager;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;
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
    public async Task SendNewAccountToManagerCommandHandler_OnHandle_ShouldEmailNameToManager([Frozen] IEventDistributor distributor, [Frozen]IQueryHandler<AccountNameQuery, AccountNameQuery.Result> query, string name, SendNewAccountToManagerCommand command, SendNewAccountToManagerCommandHandler sut)
    {
        // Arrange
        A.CallTo(() => query.Handle(A<AccountNameQuery>.That.Matches(q => q.AccountNumber == command.AccountNumber)))
            .Returns(new AccountNameQuery.Result() { AccountName = name });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<NewAccountMailedToManagerEvent>.That.Matches(e =>
                e.AccountNumber == command.AccountNumber && e.Content.Contains(name))))
            .MustHaveHappenedOnceExactly();
    }
}