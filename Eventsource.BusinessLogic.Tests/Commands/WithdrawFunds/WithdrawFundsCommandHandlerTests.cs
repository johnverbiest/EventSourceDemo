using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.WithdrawFunds;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Events.FundsWithdrawnCancelled;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Commands.WithdrawFunds;

public class WithdrawFundsCommandHandlerTests
{
    [Theory, UnitTest]
    public async Task Withdrawal_WithEnoughFunds_ShouldEmitFundsWithdrawnEvent([Frozen] IEventDistributor distributor, [Frozen] IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> queryHandler, WithdrawFundsCommand command, WithdrawFundsCommandHandler sut)
    {
        // Arrange
        command.Amount = 10;
        A.CallTo(() =>
                queryHandler.Handle(A<AccountBalanceQuery>.That.Matches(x => x.AccountNumber == command.AccountNumber)))
            .Returns(new AccountBalanceQuery.Result() {Balance = command.Amount + 1});

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() =>
            distributor.Distribute(A<FundsWithdrawnEvent>.That.Matches(x =>
                x.AccountNumber == command.AccountNumber && x.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task Withdrawal_WithJustEnoughFunds_ShouldEmitFundsWithdrawnEvent([Frozen] IEventDistributor distributor, [Frozen] IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> queryHandler, WithdrawFundsCommand command, WithdrawFundsCommandHandler sut)
    {
        // Arrange
        command.Amount = 10;
        A.CallTo(() =>
                queryHandler.Handle(A<AccountBalanceQuery>.That.Matches(x => x.AccountNumber == command.AccountNumber)))
            .Returns(new AccountBalanceQuery.Result() { Balance = command.Amount });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() =>
                distributor.Distribute(A<FundsWithdrawnEvent>.That.Matches(x =>
                    x.AccountNumber == command.AccountNumber && x.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task Withdrawal_WithInsufficientFunds_ShouldOnlyEmitFundsWithdrawalCancelledEvent([Frozen] IEventDistributor distributor, [Frozen] IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> queryHandler, WithdrawFundsCommand command, WithdrawFundsCommandHandler sut)
    {
        // Arrange
        command.Amount = 10;
        A.CallTo(() =>
                queryHandler.Handle(A<AccountBalanceQuery>.That.Matches(x => x.AccountNumber == command.AccountNumber)))
            .Returns(new AccountBalanceQuery.Result() { Balance = command.Amount - 1 });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() =>
                distributor.Distribute(A<FundsWithdrawnEvent>.That.Matches(x =>
                    x.AccountNumber == command.AccountNumber && x.Amount == command.Amount)))
            .MustNotHaveHappened();
        A.CallTo(() =>
                distributor.Distribute(A<FundsWithdrawalCancelledEvent>.That.Matches(x =>
                    x.AccountNumber == command.AccountNumber && 
                    x.Amount == command.Amount && 
                    x.Reason == "Insufficient Funds")))
            .MustHaveHappenedOnceExactly();
    }
}