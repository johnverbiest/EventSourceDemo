using System;

namespace Eventsource.BusinessLogic.Events.AccountCreated
{
    public class AccountCreatedEvent: AbstractBusinessLogicEvent
    {
        public override int AccountNumber { get; set; }
        public string Name { get; set; }
    }
}