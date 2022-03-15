using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.DepositFunds;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Commands.DepositFunds;

public class DepositFundsCommandHandlerTests
{
    [Theory, UnitTest]
    public async Task DepositFundsCommandHandler_Should_EmitFundsDepositedEvent([Frozen] IEventDistributor distributor, DepositFundsCommand command, DepositFundsCommandHandler sut)
    {
        // Arrange

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => distributor.Distribute(
                A<FundsDepositedEvent>.That.Matches(e =>
                    e.AccountNumber == command.AccountNumber && e.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
    }
}