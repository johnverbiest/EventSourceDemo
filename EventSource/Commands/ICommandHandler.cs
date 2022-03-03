using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Commands
{
    internal interface ICommandHandler<T> where T : ICommand
    {
        void Execute(T command);
    }
}
