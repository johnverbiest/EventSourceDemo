using EventSource.Events;
using EventSource.Outputs;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EventSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This is the event store
            var events = new List<IAwesomeEvent>();

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

            // this is send to a commandhandler
            // Normally this looks like _commandhandler.handle(command), but for the sake of the demo i made it easy for me
            var handler = new Commands.CreateNewPersonCommandHandler(events);
            handler.Execute(command);
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join(", ", events.GetPersons())}");
            


            
            Console.WriteLine();
            Console.WriteLine("Only one person in our pool is rather boring, lets add 3 more");
            Console.ReadLine();
            events.AddMorePeople();
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join(", ", events.GetPersons())}");
            
            
            
            
            Console.WriteLine();
            Console.WriteLine("I made a mistake in naming the 4th person, Changing the name using an PersonNameUpdatedEvent");
            Console.ReadLine();
            events.AddAndRun(new Events.PersonNameUpdatedEvent()
            {
                PersonId = 4,
                Name = "van Dyne",
                FirstName = "Janet"
            });
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join(", ", events.GetPersons())}");
            Console.WriteLine();





            Console.WriteLine();
            Console.WriteLine("It seems that mister Stark won't be joining us, let's remove him with an PersonDeletedEvent");
            Console.ReadLine();
            events.AddAndRun(new Events.PersonDeletedEvent()
            {
                PersonId = 2
            });
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join(", ", events.GetPersons())}");
            Console.WriteLine();
        }
    }
}
