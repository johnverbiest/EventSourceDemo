using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.BusinessLogic.EventStore
{
    public class SaveItAllPolicy: IEventHandler<IBusinessLogicEvent>
    {
        private readonly IEventPersistance _persistance;

        public SaveItAllPolicy(IEventPersistance persistance)
        {
            _persistance = persistance;
        }

        public Task Handle(IBusinessLogicEvent @event)
        {
            return _persistance.SaveEvent(@event);
        }
    }
}