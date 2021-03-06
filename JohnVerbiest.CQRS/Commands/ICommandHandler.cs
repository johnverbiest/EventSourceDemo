using System.Threading.Tasks;
using JohnVerbiest.CQRS.Common;

namespace JohnVerbiest.CQRS.Commands
{
    public interface ICommandHandler<in T> : IHaveSingleHandler where T: ICommand
    {
        Task ExecuteAsync(T command);
    }
}