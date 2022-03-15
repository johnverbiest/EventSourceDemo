using System;

namespace Eventsource.BusinessLogic.Events.AccountCreated
{
    public class AccountCreatedEvent: AbstractBusinessLogicEvent
    {
        public int AccountNumber { get; set; }
        public string Name { get; set; }       
    }
}