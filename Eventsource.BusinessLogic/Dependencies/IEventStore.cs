using System;
using System.Threading.Tasks;
using Eventsource.BusinessLogic.Events;

namespace Eventsource.BusinessLogic.Dependencies
{
    public interface IEventStore
    {
        Task SaveEvent<T>(T @event) where T: IBusinessLogicEvent;
        Task<IBusinessLogicEvent[]> LoadEvents(int[] accounts, params Type[] eventTypes);
    }
}