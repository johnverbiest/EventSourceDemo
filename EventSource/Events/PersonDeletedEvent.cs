using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class PersonDeletedEvent : BaseEvent
    {

        public int PersonId { get; set; }

        public PersonDeletedEvent(): base() { }

    }
}
