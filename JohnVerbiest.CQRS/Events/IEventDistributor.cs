using System.Threading.Tasks;

namespace JohnVerbiest.CQRS.Events
{
    public interface IEventDistributor
    {
        Task Distribute<T>(T @event) where T: IEvent;
    }
}