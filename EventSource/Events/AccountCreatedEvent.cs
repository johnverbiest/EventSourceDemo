using EventSource.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Events
{
    internal class AccountCreatedEvent: BaseEvent
    {
        public AccountCreatedEvent(Dto.Account account): base()
        {
            Account = account;
        }

        public Account Account { get; }
    }
}
