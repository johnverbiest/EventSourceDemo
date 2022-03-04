using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class AccountDeletedEvent: BaseEvent
    {
        public AccountDeletedEvent(): base()
        {
        }

        public int AccountId { get; set; }
    }
}
