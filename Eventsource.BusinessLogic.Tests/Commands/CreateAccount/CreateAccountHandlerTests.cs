using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.CreateAccount;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Commands.CreateAccount;

public class CreateAccountHandlerTests
{
    [Theory, UnitTest]
    public async Task CreateAccountCommandHandler_OnAccountCreation_ThrowCreatedAccountEvent([Frozen] IEventDistributor distributor, CreateAccountCommand command, CreateAccountCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<AccountCreatedEvent>._)).MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task CreateAccountCommandHandler_OnAccountCreation_ShouldEnterNameInEvent([Frozen] IEventDistributor distributor, CreateAccountCommand command, CreateAccountCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<AccountCreatedEvent>.That.Matches(x => x.Name == command.Name))).MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task CreateAccountCommandHandler_OnFirstAccountCreation_ShouldHaveAccount1([Frozen] IEventDistributor distributor, CreateAccountCommand command, CreateAccountCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<AccountCreatedEvent>.That.Matches(x => x.AccountNumber == 1))).MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task CreateAccountCommandHandler_OnSubsequentAccountCreation_AccountNumberShoudBeMaxPlusOne([Frozen] IEventDistributor distributor, [Frozen] IQueryHandler<HighestAccountNumberQuery, HighestAccountNumberQuery.Result> query, int highestAccountNumber, CreateAccountCommand command, CreateAccountCommandHandler sut)
    {
        // Arrange
        A.CallTo(() => query.Handle(A<HighestAccountNumberQuery>._)).Returns(new HighestAccountNumberQuery.Result() { HighestAccountNumber = highestAccountNumber});

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<AccountCreatedEvent>.That.Matches(x => x.AccountNumber == highestAccountNumber + 1))).MustHaveHappenedOnceExactly();
    }
}