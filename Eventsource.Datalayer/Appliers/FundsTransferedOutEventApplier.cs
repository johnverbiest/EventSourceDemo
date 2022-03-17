using Eventsource.BusinessLogic.Events.FundsTransfered;

namespace Eventsource.Datalayer.Appliers;

public static class FundsTransferedOutEventApplier
{
    public static decimal Apply(this FundsTransferedOutEvent @event, decimal oldValue) => oldValue - @event.Amount;
}