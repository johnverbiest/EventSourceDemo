using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsTransferedIn;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer.QueryHandlers;
using FluentAssertions;
using Xunit;

namespace Eventsource.Datalayer.Tests.QueryHandlers;

public class AccountBalanceQueryHandlerTests
{
    [Theory, UnitTest]
    public async Task AccountBalanceQueryHandler_Should_ProcessDepositFunds([Frozen] IEventStore store, int accountNumber, FundsDepositedEvent event1, FundsDepositedEvent event2, FundsDepositedEvent event3 , AccountBalanceQuery query, AccountBalanceQueryHandler sut)
    {
        // Arrange
        event1.AccountNumber = accountNumber + 2;
        event2.AccountNumber = accountNumber;
        event3.AccountNumber = accountNumber;
        query.AccountNumber = accountNumber;

        event2.Amount = 5;
        event3.Amount = 15;
        var expectedResult = 20;
        store.InjectEvents(event1, event2, event3);


        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Balance.Should().Be(expectedResult);
    }

    [Theory, UnitTest]
    public async Task AccountBalanceQueryHandler_Should_ProcessWithdrawFunds([Frozen] IEventStore store, int accountNumber, FundsDepositedEvent event1, FundsDepositedEvent event2, FundsWithdrawnEvent event3, AccountBalanceQuery query, AccountBalanceQueryHandler sut)
    {
        // Arrange
        event1.AccountNumber = accountNumber + 2;
        event2.AccountNumber = accountNumber;
        event3.AccountNumber = accountNumber;
        query.AccountNumber = accountNumber;

        event2.Amount = 10;
        event3.Amount = 1;
        var expectedResult = 9;
        store.InjectEvents(event1, event2, event3);


        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Balance.Should().Be(expectedResult);
    }

    [Theory, UnitTest]
    public async Task AccountBalanceQueryHandler_Should_ProcessOutgoingTransferToOtherFund([Frozen] IEventStore store, int accountNumber, FundsDepositedEvent event1, FundsDepositedEvent event2, FundsTransferedOutEvent event3, AccountBalanceQuery query, AccountBalanceQueryHandler sut)
    {
        // Arrange
        event1.AccountNumber = accountNumber + 2;
        event2.AccountNumber = accountNumber;
        event3.AccountNumber = accountNumber;
        query.AccountNumber = accountNumber;

        event2.Amount = 15;
        event3.Amount = 5;
        var expectedResult = 10;
        store.InjectEvents(event1, event2, event3);


        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Balance.Should().Be(expectedResult);
    }

    [Theory, UnitTest]
    public async Task AccountBalanceQueryHandler_Should_ProcessIncomingTransferFromOtherFund([Frozen] IEventStore store, int accountNumber, FundsDepositedEvent event1, FundsDepositedEvent event2, FundsTransferedInEvent event3, AccountBalanceQuery query, AccountBalanceQueryHandler sut)
    {
        // Arrange
        event1.AccountNumber = accountNumber + 2;
        event2.AccountNumber = accountNumber;
        event3.AccountNumber = accountNumber + 2;
        event3.AccountNumber = accountNumber;
        query.AccountNumber = accountNumber;

        event2.Amount = 15;
        event3.Amount = 5;
        var expectedResult = 20;
        store.InjectEvents(event1, event2, event3);


        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Balance.Should().Be(expectedResult);
    }
}