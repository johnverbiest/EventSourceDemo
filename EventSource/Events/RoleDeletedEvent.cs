using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class RoleDeletedEvent: BaseEvent
    {
        public RoleDeletedEvent(): base()
        {
        }

        public int RoleId { get; set; }
    }
}
