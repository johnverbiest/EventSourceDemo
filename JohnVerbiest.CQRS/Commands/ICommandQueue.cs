using System.Threading.Tasks;

namespace JohnVerbiest.CQRS.Commands
{
    public interface ICommandQueue
    {
        Task QueueForExecution<T> (T command) where T: ICommand;
    }
}