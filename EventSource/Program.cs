using EventSource.Events;
using EventSource.Outputs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace EventSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This is the event store
            var events = new List<IAwesomeEvent>();
            events.RunSetupPart2();
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join(", ", events.GetPersons())}");
            Console.WriteLine();




            Console.WriteLine();
            Console.WriteLine("Now that we have our mini-avenger team, let's create an account for Peter Parker");
            Console.ReadLine();
            events.AddAndRun(new Events.AccountCreatedEvent(new Dto.Account
            {
                AccountId = 1,
                PersonId = 3,
                Username = "The Amazing",
                Password = "Spiderman"
                
            }));
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join("\n", events.GetPersonsWithAccounts())}");
            Console.WriteLine();




            Console.WriteLine();
            Console.WriteLine("We need more accounts to work with... let's do this!");
            Console.ReadLine();
            events.AddMoreAccounts();
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join("\n", events.GetPersonsWithAccounts())}");
            Console.WriteLine();




            Console.WriteLine();
            Console.WriteLine("Silly me! Peter now has 2 accounts and Rogers wants another username... Lets's fix this with a AccountUsernameUpdatedEvent and a AccountDeletedEvent ");
            Console.ReadLine();
            events.AddAndRun(new Events.AccountUsernameUpdatedEvent()
            {
                AccountId=3,
                UserName="Captain America"
            });

            events.AddAndRun(new Events.AccountDeletedEvent()
            {
                AccountId=2
            });
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join("\n", events.GetPersonsWithAccounts())}");
            Console.WriteLine();





            Console.WriteLine();
            Console.WriteLine("Sadly, our Wasp does not langer wants to play with us, let's delete her from the system");
            Console.ReadLine();
            events.AddAndRun(new Events.PersonDeletedEvent()
            {
                PersonId=4
            });
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join("\n", events.GetPersonsWithAccounts())}");
            Console.WriteLine();




            Console.WriteLine();
            Console.WriteLine("Let's look at the final result of the 'read only' database");
            Console.ReadLine();
            Console.Clear();
            var eventsInStupidOrder = events.OrderBy(x => x.Id).ToList();
            var timer = Stopwatch.StartNew();
            var data = eventsInStupidOrder.RunAllEvents();
            timer.Stop();
            eventsInStupidOrder.PrintEvents();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine();
            Console.WriteLine($"Replay done in {timer.ElapsedMilliseconds} ms");
        }
    }

}
