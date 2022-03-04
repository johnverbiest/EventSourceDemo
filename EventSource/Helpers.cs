using EventSource.Events;
using EventSource.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource
{
    internal static class Helpers
    {
        public static void AddMorePeople(this List<IAwesomeEvent> events)
        {
            events.AddAndRun(new PersonCreatedEvent(new Dto.Person { Id = 2, Name = "Stark", Firstname = "Tony", DateOfBirth=DateTime.Now.AddYears(-38) }));
            events.AddAndRun(new PersonCreatedEvent(new Dto.Person { Id = 3, Name = "Parker", Firstname = "Peter", DateOfBirth = DateTime.Now.AddYears(-19) }));
            events.AddAndRun(new PersonCreatedEvent(new Dto.Person { Id = 4, Name = "Wasp", Firstname = "The", DateOfBirth = DateTime.Now.AddYears(-27) }));
        }

        public static void RunSetupPart2(this List<IAwesomeEvent> events)
        {
            // A user adds a person
            var command = new Commands.CreateNewPersonCommand()
            {
                Person = new Dto.Person
                {
                    Id = 1,
                    Name = "Rogers",
                    Firstname = "Steven",
                    DateOfBirth = new DateTime(1903, 2, 8)
                }
            };

            var handler = new Commands.CreateNewPersonCommandHandler(events);
            handler.Execute(command);
            events.AddMorePeople();
            events.AddAndRun(new Events.PersonNameUpdatedEvent()
            {
                PersonId = 4,
                Name = "van Dyne",
                FirstName = "Janet"
            });
            events.AddAndRun(new Events.PersonDeletedEvent()
            {
                PersonId = 2
            });
        }

        public static void AddMoreAccounts(this List<IAwesomeEvent> events)
        {
            events.AddAndRun(new Events.AccountCreatedEvent(new Dto.Account
            {
                AccountId = 2,
                PersonId = 3,
                Username = "Second Account for the amazing",
                Password = "Spiderman"
            }));
            events.AddAndRun(new Events.AccountCreatedEvent(new Dto.Account
            {
                AccountId = 3,
                PersonId = 1,
                Username = "Captain",
                Password = "America"
            }));
            events.AddAndRun(new Events.AccountCreatedEvent(new Dto.Account
            {
                AccountId = 4,
                PersonId = 4,
                Username = "The Wasp",
                Password = "Bzzzz"
            }));
        }
    }
}
