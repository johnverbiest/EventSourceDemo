using System.Threading.Tasks;
using JohnVerbiest.CQRS.Common;

namespace JohnVerbiest.CQRS.Events
{
    public interface IEventHandler<in TEvent> : IHaveMultipleHandlers where TEvent: IEvent
    {
        Task Handle(TEvent @event);
    }
}