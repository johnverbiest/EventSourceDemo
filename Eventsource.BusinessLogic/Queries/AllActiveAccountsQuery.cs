using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery
{
    public class AllActiveAccountsQuery: IQuery<AllActiveAccountsQuery.Result>
    {
        public record Account
        {
            public int AccountNumber { get; set; }
            public string Name { get; set; }
        }

        public class Result
        {
            public Account[] Accounts { get; set; }
        }
    }
}