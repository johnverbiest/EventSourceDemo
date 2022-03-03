using EventSource.Events;
using EventSource.Outputs;
using System.Collections.Generic;

namespace EventSource.Commands
{
    internal class CreateNewPersonCommandHandler : ICommandHandler<CreateNewPersonCommand>
    {
        private readonly List<IAwesomeEvent> events;

        public CreateNewPersonCommandHandler(List<IAwesomeEvent> events)
        {
            this.events = events;
        }

        public void Execute(CreateNewPersonCommand command)
        {
            events.AddAndRun(new PersonCreatedEvent(command.Person));
        }
    }
}
