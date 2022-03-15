using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer.QueryHandlers;
using FluentAssertions;
using Xunit;

namespace Eventsource.Datalayer.Tests.QueryHandlers;

public class AllActiveAccountsQueryHandlerTests
{
    [Theory, UnitTest]
    public async Task AllActiveAccountsQueryHandler_Should_ProcessAccountCreatedEvents([Frozen] IEventStore store, AccountCreatedEvent @event, AllActiveAccountsQuery query, AllActiveAccountsQueryHandler sut)
    {
        // Arrange
        store.InjectEvents(@event);

        // Act
        var result = await sut.Handle(query);

        // Assert
        result.Accounts.Should().HaveCount(1);
        result.Accounts[0].AccountNumber.Should().Be(@event.AccountNumber);
        result.Accounts[0].Name.Should().Be(@event.Name);
    }
}