using Eventsource.BusinessLogic.Events.FundsTransferedIn;

namespace Eventsource.Datalayer.Appliers;

public static class FundsTransferedInEventApplier
{
    public static decimal Apply(this FundsTransferedInEvent @event, decimal oldBalance) => oldBalance + @event.Amount;
}