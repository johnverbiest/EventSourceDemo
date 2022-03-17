using Eventsource.BusinessLogic.Events.FundsDeposited;

namespace Eventsource.Datalayer.Appliers;

public static class FundsDepositedEventApplier
{
    public static decimal Apply(this FundsDepositedEvent @event, decimal oldBalance) => oldBalance + @event.Amount;
}