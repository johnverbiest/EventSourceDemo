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
    }
}
