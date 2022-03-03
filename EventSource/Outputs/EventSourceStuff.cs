using EventSource.Dto;
using EventSource.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.Outputs
{
    internal static class EventSourceStuff
    {

        public class EventSourceData
        {
            public List<Person> Person { get; set; } = new List<Person>();
        }
        private static readonly EventSourceData cachedData = new();


        /// <summary>
        /// 
        ///  Run a full resync of the "data model"
        ///  
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static EventSourceData RunAllEvents(this List<IAwesomeEvent> events)
        {
            // Start from 0
            var cachedData = new EventSourceData();


            // Process each event
            foreach (var ev in events.OrderBy(x => x.FiredAt))
            {
                cachedData.Run(ev);
            }


            // Return the data
            return cachedData;
        }





        /// <summary>
        /// Adds the event to the store and run it on the cached data model
        /// </summary>
        /// <param name="events"></param>
        /// <param name="ev"></param>
        public static void AddAndRun(this List<IAwesomeEvent> events, IAwesomeEvent ev)
        {
            events.Add(ev);
            cachedData.Run(ev);
        }





        /// <summary>
        /// Print all the events
        /// </summary>
        /// <param name="events"></param>
        public static void PrintEvents(this List<IAwesomeEvent> events)
        {
            Console.Clear();
            Console.WriteLine("Current Events In Store:");
            Console.WriteLine("----------------------------");
            foreach (var e in events)
            {
                Console.WriteLine(e.ToJson());
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine($"Total: {events.Count} events");
        }




        /// <summary>
        /// Process an event
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ev"></param>
        public static void Run(this EventSourceData data, IAwesomeEvent ev)
        {
            switch (ev)
            {
                case PersonCreatedEvent e:
                    data.Person.Add(e.Person);
                    break;

                case PersonNameUpdatedEvent e:
                    var person = data.Person.Single(x => x.Id == e.PersonId);
                    person.Name = e.Name;
                    person.Firstname = e.FirstName;
                    break;

                case PersonDeletedEvent e:
                    data.Person.RemoveAll(x => x.Id == e.PersonId);
                    break;
            }
        }



        /// <summary>
        /// Return the persons from the cached data
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPersons(this List<IAwesomeEvent> events)
        {
            return cachedData.Person.Select(e => e.Name);
        }
    }
}
