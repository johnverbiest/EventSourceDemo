using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class RoleCreatedEvent: BaseEvent
    {
        public RoleCreatedEvent(Dto.Role role): base()
        {
            Role = role;
        }

        public Role Role { get; }
    }
}
