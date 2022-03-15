using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.Datalayer.EventHandler
{
    public class SaveItAllPolicy: IEventHandler<IBusinessLogicEvent>
    {
        private readonly IEventStore _store;

        public SaveItAllPolicy(IEventStore store)
        {
            _store = store;
        }

        public Task Handle(IBusinessLogicEvent @event)
        {
            return _store.SaveEvent(@event);
        }
    }
}