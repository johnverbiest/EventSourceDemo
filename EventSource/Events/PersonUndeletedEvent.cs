using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class PersonUndeletedEvent : BaseEvent
    {
        public PersonUndeletedEvent() : base()
        {
        }

        public Guid EventId { get; set; }
    }
}
