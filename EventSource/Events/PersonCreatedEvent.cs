using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class PersonCreatedEvent : BaseEvent
    {
        public Person Person { get; set; }

        public PersonCreatedEvent(Person person) : base()
        {
            Person = person;
        }

    }
}
