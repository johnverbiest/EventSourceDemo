using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer.QueryHandlers;
using FluentAssertions;
using Xunit;

namespace Eventsource.Datalayer.Tests.QueryHandlers;

public class AccountNameQueryHandlerTests
{
    [Theory, UnitTest]
    public async Task AccountNameQueryHandler_Should_ProcessAccountCreatedEvents([Frozen] IEventStore store, AccountCreatedEvent @event, int expectedResult, AccountCreatedEvent event2, AccountCreatedEvent event3, HighestAccountNumberQuery query, HighestAccountNumberQueryHandler sut)
    {
        // Arrange
        @event.AccountNumber = expectedResult;
        event2.AccountNumber = expectedResult - 10;
        event3.AccountNumber = expectedResult - 3;
        store.InjectEvents(@event, event2, event3);
        

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.HighestAccountNumber.Should().Be(expectedResult);
    }
}