using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.CreateAccount;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.Events.AccountCreated;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
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
    public async Task CreateAccountCommandHandler_OnSubsequentAccountCreation_AccountNumberShoudBeMaxPlusOne([Frozen] IEventDistributor distributor, [Frozen] IEventPersistance eventStore, AccountCreatedEvent initialEvent, AccountCreatedEvent initialEvent2, CreateAccountCommand command, CreateAccountCommandHandler sut)
    {
        // Arrange
        initialEvent2.AccountNumber = initialEvent.AccountNumber - 20;
        var expectedAccountNumber = initialEvent.AccountNumber + 1;
        A.CallTo(() => eventStore.LoadEvents()).WithAnyArguments().Returns(new[] { initialEvent, initialEvent2 });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(A<AccountCreatedEvent>.That.Matches(x => x.AccountNumber == expectedAccountNumber))).MustHaveHappenedOnceExactly();
    }
}