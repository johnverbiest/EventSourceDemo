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
            public List<Person> Persons { get; set; } = new List<Person>();
            public List<Account> Accounts { get; set; } = new List<Account>();
            public List<Role> Roles { get; set; } = new List<Role>();
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
            var data = new EventSourceData();


            // Process each event
            foreach (var ev in events.OrderBy(x => x.FiredAt))
            {
                data.Run(ev, events);
            }


            // Return the data
            return data;
        }





        /// <summary>
        /// Adds the event to the store and run it on the cached data model
        /// </summary>
        /// <param name="events"></param>
        /// <param name="ev"></param>
        public static void AddAndRun(this List<IAwesomeEvent> events, IAwesomeEvent ev)
        {
            events.Add(ev);
            cachedData.Run(ev, events);
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
        public static void Run(this EventSourceData data, IAwesomeEvent ev, List<IAwesomeEvent> events)
        {
            switch (ev)
            {
                case PersonCreatedEvent e:
                    AddPerson(data, e);
                    break;

                case PersonNameUpdatedEvent e:
                    UpdatePersonName(data, e);
                    break;

                case PersonDeletedEvent e:
                    DeletePerson(data, e);
                    DeleteAccount(data, e);
                    break;
                case PersonUndeletedEvent e:
                    UndeletePerson(data, events, e);
                    break;


                case AccountCreatedEvent e:
                    AddAccount(data, e);
                    break;

                case AccountUsernameUpdatedEvent e:
                    UpdateAccountUsername(data, e);
                    break;

                case AccountDeletedEvent e:
                    DeleteAccount(data, e);
                    break;


                case RoleCreatedEvent e:
                    AddRole(data, e);
                    break;

                case RoleNameUpdatedEvent e:
                    UpdateRoleName(data, e);
                    break;

                case RoleDeletedEvent e:
                    DeleteRole(data, e);
                    break;
            }
        }

        private static void DeleteRole(EventSourceData data, RoleDeletedEvent e)
        {
            data.Roles.RemoveAll(x => x.Id == e.RoleId);
        }

        private static void UpdateRoleName(EventSourceData data, RoleNameUpdatedEvent e)
        {
            var role = data.Roles.Single(x => x.Id == e.RoleId);
            data.Roles.Remove(role);
            var newrole = new Role
            {
                Id = role.Id,
                Name = e.Name,
            };
            data.Roles.Add(newrole);
        }

        private static void AddRole(EventSourceData data, RoleCreatedEvent e)
        {
            data.Roles.Add(e.Role);
        }

        private static void UndeletePerson(EventSourceData data, List<IAwesomeEvent> events, PersonUndeletedEvent e)
        {
            var deleteEvent = events.OfType<PersonDeletedEvent>().Single(x => x.Id == e.EventId);
            var rebuildEvents = events.Where(x => x.FiredAt < deleteEvent.FiredAt).ToList();
            var stateBeforeDeletion = rebuildEvents.RunAllEvents();

            var person = stateBeforeDeletion.Persons.Single(x => x.Id == deleteEvent.PersonId);
            var accounts = stateBeforeDeletion.Accounts.Where(x => x.PersonId == person.Id);

            data.Persons.Add(person);
            data.Accounts.AddRange(accounts);
        }

        private static void DeleteAccount(EventSourceData data, AccountDeletedEvent e)
        {
            data.Accounts.RemoveAll(x => x.AccountId == e.AccountId);
        }

        private static void DeleteAccount(EventSourceData data, PersonDeletedEvent e)
        {
            data.Accounts.RemoveAll(x => x.PersonId == e.PersonId);
        }

        private static void UpdateAccountUsername(EventSourceData data, AccountUsernameUpdatedEvent e)
        {
            var account = data.Accounts.Single(x => x.AccountId == e.AccountId);
            data.Accounts.Remove(account);
            var newaccount = new Account
            {
                AccountId = account.AccountId,
                Username = e.UserName,
                Password = account.Password,
                PersonId = account.PersonId

            };
            data.Accounts.Add(newaccount);
        }

        private static void AddAccount(EventSourceData data, AccountCreatedEvent e)
        {
            data.Accounts.Add(e.Account);
        }

        private static void DeletePerson(EventSourceData data, PersonDeletedEvent e)
        {
            data.Persons.RemoveAll(x => x.Id == e.PersonId);
        }

        private static void UpdatePersonName(EventSourceData data, PersonNameUpdatedEvent e)
        {
            var person = data.Persons.Single(x => x.Id == e.PersonId);
            data.Persons.Remove(person);
            var newperson = new Person
            {
                Id = person.Id,
                Name = e.Name,
                Firstname = e.FirstName,
                DateOfBirth = person.DateOfBirth
            };
            data.Persons.Add(newperson);
        }

        private static void AddPerson(EventSourceData data, PersonCreatedEvent e)
        {
            data.Persons.Add(e.Person);
        }



        /// <summary>
        /// Return the persons from the cached data
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPersons(this List<IAwesomeEvent> events)
        {
            return cachedData.Persons.Select(e => e.Name);
        }


        public static IEnumerable<string> GetPersonsWithAccounts(this List<IAwesomeEvent> events)
        {
            return cachedData.Persons.Select(p => $"\nPERSON: {p.Name}, {p.Firstname}: \n{string.Join("\n", cachedData.Accounts.Where(a => a.PersonId == p.Id).Select(a => " - ACCOUNT username:" + a.Username))}");
        }
    }
}
