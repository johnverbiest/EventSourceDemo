using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class AccountUsernameUpdatedEvent: BaseEvent
    {
        public AccountUsernameUpdatedEvent(): base()
        {
        }

        public int AccountId { get; set; }
        public string UserName { get; set; }
    }
}
