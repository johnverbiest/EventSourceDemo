using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class PersonNameUpdatedEvent : BaseEvent
    {

        public int PersonId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }

        public PersonNameUpdatedEvent(): base() { }

    }
}
