using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using Eventsource.BusinessLogic.Events.FundsTranferCancelled;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsTransferedIn;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.BusinessLogic.Events.FundsWithdrawnCancelled;
using Eventsource.BusinessLogic.Events.WelcomeMailSent;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer.QueryHandlers;
using FluentAssertions;
using Xunit;

namespace Eventsource.Datalayer.Tests.QueryHandlers;

public class AccountHistoryQueryHandlerTests
{
    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessAccountCreated([Frozen] IEventStore store, AccountCreatedEvent testEvent, AccountCreatedEvent otherEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testEvent.AccountNumber;
        otherEvent.AccountNumber = testEvent.AccountNumber + 1;
        store.InjectEvents(testEvent, otherEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == 0 && x.date == testEvent.EventRaised && x.Description == "Account Created");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessFundsDeposited([Frozen] IEventStore store, FundsDepositedEvent testEvent, FundsDepositedEvent otherEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testEvent.AccountNumber;
        otherEvent.AccountNumber = testEvent.AccountNumber + 1;
        store.InjectEvents(testEvent, otherEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == testEvent.Amount && x.date == testEvent.EventRaised && x.Description == $"Funds Deposited: {testEvent.Amount}");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessFundsTransferCancelled([Frozen] IEventStore store, FundStranferCancelledEvent testEvent, FundStranferCancelledEvent otherEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testEvent.AccountNumber;
        otherEvent.AccountNumber = testEvent.AccountNumber + 1;
        store.InjectEvents(testEvent, otherEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == 0 && x.date == testEvent.EventRaised && x.Description == $"Funds Transfer of {testEvent.Amount} to account {testEvent.DestinationAccountNumber} Failed: {testEvent.Reason}");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessFundsTransferred([Frozen] IEventStore store, FundsTransferedOutEvent testOutEvent, FundsTransferedOutEvent otherOutEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testOutEvent.AccountNumber;
        otherOutEvent.AccountNumber = testOutEvent.AccountNumber + 1;
        store.InjectEvents(testOutEvent, otherOutEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == -testOutEvent.Amount && x.date == testOutEvent.EventRaised && x.Description == $"Outgoing Funds Transfer of {testOutEvent.Amount} to account {testOutEvent.DestinationAccountNumber}");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessIncomingFundsTransferred([Frozen] IEventStore store, FundsTransferedInEvent testOutEvent, FundsTransferedInEvent otherOutEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testOutEvent.AccountNumber;
        otherOutEvent.AccountNumber = testOutEvent.AccountNumber + testOutEvent.AccountNumber + 2;
        store.InjectEvents(testOutEvent, otherOutEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == testOutEvent.Amount && x.date == testOutEvent.EventRaised && x.Description == $"Incoming Funds Transfer of {testOutEvent.Amount} from account {testOutEvent.OriginAccountNumber}");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessFundsWithdrawn([Frozen] IEventStore store, FundsWithdrawnEvent testEvent, FundsWithdrawnEvent otherEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testEvent.AccountNumber;
        otherEvent.AccountNumber = testEvent.AccountNumber + 1;
        store.InjectEvents(testEvent, otherEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == -testEvent.Amount && x.date == testEvent.EventRaised && x.Description == $"Funds withdrawn: {testEvent.Amount}");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessFundsWithdrawnCancelled([Frozen] IEventStore store, FundsWithdrawalCancelledEvent testEvent, FundsWithdrawalCancelledEvent otherEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testEvent.AccountNumber;
        otherEvent.AccountNumber = testEvent.AccountNumber + 1;
        store.InjectEvents(testEvent, otherEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == 0 && x.date == testEvent.EventRaised && x.Description == $"Funds withdrawal of {testEvent.Amount} failed: {testEvent.Reason}");
    }

    [Theory, UnitTest]
    public async Task AccountHistoryQuery_Should_ProcessWelcomeMailSent([Frozen] IEventStore store, WelcomeMailSentEvent testEvent, WelcomeMailSentEvent otherEvent, AccountHistoryQuery query, AccountHistoryQueryHandler sut)
    {
        // Arrange
        query.AccountNumber = testEvent.AccountNumber;
        otherEvent.AccountNumber = testEvent.AccountNumber + 1;
        store.InjectEvents(testEvent, otherEvent);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Events.Should().OnlyContain(x =>
            x.Balance == 0 && x.date == testEvent.EventRaised && x.Description == "Welcome mail sent");
    }
}