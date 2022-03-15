using System.Linq;
using System.Threading.Tasks;
using JohnVerbiest.CQRS.Dependencies;

namespace JohnVerbiest.CQRS.Events
{
    public class EventDistributor: IEventDistributor
    {
        private readonly IHandlerRequestDependency _dependency;

        public EventDistributor(IHandlerRequestDependency dependency)
        {
            _dependency = dependency;
        }

        public async Task Distribute<T>(T @event) where T: IEvent
        {

            var objecthandlers = await _dependency.GetHandlers<IEventHandler<T>>(typeof(IEventHandler<T>));
            var threads = objecthandlers.Select(handler => handler.Handle(@event)).ToList();

            foreach (var @interface in typeof(T).GetInterfaces())
            {
                var handler = typeof(IEventHandler<>).MakeGenericType(@interface);
                var customInterfaceHandlers = await _dependency.GetHandlers<IEventHandler<T>>(handler);
                threads.AddRange(customInterfaceHandlers.Select(handler => handler.Handle(@event)));
            }

            await Task.WhenAll(threads);
        }
    }
}