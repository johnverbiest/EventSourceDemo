using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class RoleNameUpdatedEvent: BaseEvent
    {
        public RoleNameUpdatedEvent(): base()
        {
        }

        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}
