using System.Threading.Tasks;
using JohnVerbiest.CQRS.Dependencies;

namespace JohnVerbiest.CQRS.Commands
{
    public class CommandQueue: ICommandQueue
    {
        private readonly IHandlerRequestDependency _handlerFetcher;

        public CommandQueue(IHandlerRequestDependency handlerFetcher)
        {
            _handlerFetcher = handlerFetcher;
        }

        public async Task QueueForExecution<T>(T command) where T : ICommand
        {
            var handler = await _handlerFetcher.GetHandler<ICommandHandler<T>>();
            await handler.ExecuteAsync(command);
        }
    }
}