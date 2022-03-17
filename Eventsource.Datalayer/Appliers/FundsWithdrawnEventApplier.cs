using Eventsource.BusinessLogic.Events.FundsWithdrawn;

namespace Eventsource.Datalayer.Appliers;

public static class FundsWithdrawnEventApplier
{
    public static decimal Apply(this FundsWithdrawnEvent @event, decimal oldBalance) => oldBalance - @event.Amount;
}