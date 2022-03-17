using Eventsource.BusinessLogic.Events.AccountCreated;

namespace Eventsource.Datalayer.Appliers;

public static class AccountCreatedEventApplier
{
    public static (int accountNumber, string accountName) Apply(this AccountCreatedEvent @event) =>
        (@event.AccountNumber, @event.Name);
}