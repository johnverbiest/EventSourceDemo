using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource
{
    interface IAwesomeEventHandler<T> where T: IAwesomeEvent
    {
        void handle(T value);
    }
}
