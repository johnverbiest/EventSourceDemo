using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Commands
{
    internal class CreateNewPersonCommand: ICommand
    {
        public Person Person { get; set; }
    }
}
