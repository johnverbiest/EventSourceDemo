using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class RoleAssignedEvent: BaseEvent
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
    }
}
