using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Commands.TransferFunds;
using Eventsource.BusinessLogic.Events.FundsTranferCancelled;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsTransferedIn;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using FakeItEasy;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;
using Xunit;

namespace Eventsource.BusinessLogic.Tests.Commands.TransferFunds;

public class TransferFundsCommandHandlerTests
{
    [Theory, UnitTest]
    public async Task TransferFunds_WithEnoughFunds_ShouldEmitFundsTransferedEvent([Frozen]IEventDistributor eventDistributor, [Frozen] IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> queryHandler, TransferFundsCommand command, TransferFundsCommandHandler sut)
    {
        // Arrange
        var transferAmount = 10;
        A.CallTo(() =>
                queryHandler.Handle(A<AccountBalanceQuery>.That.Matches(x => x.AccountNumber == command.AccountNumber)))
            .Returns(new AccountBalanceQuery.Result() {Balance = transferAmount + 1 });
        command.Amount = transferAmount;

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => eventDistributor.Distribute(A<FundsTransferedOutEvent>.That.Matches(x =>
            x.AccountNumber == command.AccountNumber &&
            x.DestinationAccountNumber == command.DestinationAccountNumber && 
            x.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => eventDistributor.Distribute(A<FundsTransferedInEvent>.That.Matches(x =>
                x.AccountNumber == command.DestinationAccountNumber &&
                x.OriginAccountNumber == command.AccountNumber &&
                x.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task TransferFunds_WithJustEnoughFunds_ShouldEmitFundsTransferedEvent([Frozen] IEventDistributor eventDistributor, [Frozen] IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> queryHandler, TransferFundsCommand command, TransferFundsCommandHandler sut)
    {
        // Arrange
        var transferAmount = 10;
        A.CallTo(() =>
                queryHandler.Handle(A<AccountBalanceQuery>.That.Matches(x => x.AccountNumber == command.AccountNumber)))
            .Returns(new AccountBalanceQuery.Result() { Balance = transferAmount });
        command.Amount = transferAmount;

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => eventDistributor.Distribute(A<FundsTransferedOutEvent>.That.Matches(x =>
                x.AccountNumber == command.AccountNumber &&
                x.DestinationAccountNumber == command.DestinationAccountNumber &&
                x.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => eventDistributor.Distribute(A<FundsTransferedInEvent>.That.Matches(x =>
                x.AccountNumber == command.DestinationAccountNumber &&
                x.OriginAccountNumber == command.AccountNumber &&
                x.Amount == command.Amount)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, UnitTest]
    public async Task TransferFunds_WithInsufficientFunds_ShouldOnlyEmitFunsTransferredCancelledEvent([Frozen] IEventDistributor eventDistributor, [Frozen] IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> queryHandler, TransferFundsCommand command, TransferFundsCommandHandler sut)
    {
        // Arrange
        var transferAmount = 10;
        A.CallTo(() =>
                queryHandler.Handle(A<AccountBalanceQuery>.That.Matches(x => x.AccountNumber == command.AccountNumber)))
            .Returns(new AccountBalanceQuery.Result() { Balance = transferAmount - 1 });
        command.Amount = transferAmount;

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        A.CallTo(() => eventDistributor.Distribute(A<FundsTransferedOutEvent>.That.Matches(x =>
                x.AccountNumber == command.AccountNumber &&
                x.DestinationAccountNumber == command.DestinationAccountNumber &&
                x.Amount == command.Amount)))
            .MustNotHaveHappened();
        A.CallTo(() => eventDistributor.Distribute(A<FundsTransferedInEvent>.That.Matches(x =>
                x.AccountNumber == command.DestinationAccountNumber &&
                x.OriginAccountNumber == command.AccountNumber &&
                x.Amount == command.Amount)))
            .MustNotHaveHappened();
        A.CallTo(() => eventDistributor.Distribute(A<FundStranferCancelledEvent>.That.Matches(x =>
                x.AccountNumber == command.AccountNumber &&
                x.DestinationAccountNumber == command.DestinationAccountNumber &&
                x.Amount == command.Amount &&
                x.Reason == "Insufficient Funds")))
            .MustHaveHappenedOnceExactly();
    }
}