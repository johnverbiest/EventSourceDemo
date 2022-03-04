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
            events.RunSetupPart3();
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join(", ", events.GetPersons())}");
            Console.WriteLine();


            Console.WriteLine();
            Console.WriteLine("First let's go ahead and add some roles");
            Console.ReadLine();
            events.AddSomeRoles();
            events.PrintEvents();
            Console.WriteLine($"Getting the list of people in the pool now (cached):\n{string.Join("\n", events.GetPersonsWithAccounts())}");
            Console.WriteLine();


            Console.WriteLine();
            Console.WriteLine("Now we'll assign some roles");
            Console.ReadLine();
            var randomRolePicker = new Random(DateTime.Now.Millisecond);
            var min = 1; var max = 5;
            events.AddAndRun(new RoleAssignedEvent()
            {
                AccountId = 1,
                RoleId = randomRolePicker.Next(min, max)
            });
            events.AddAndRun(new RoleAssignedEvent()
            {
                AccountId = 3,
                RoleId = randomRolePicker.Next(min, max)
            });
            events.AddAndRun(new RoleAssignedEvent()
            {
                AccountId = 4,
                RoleId = randomRolePicker.Next(min, max)
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
