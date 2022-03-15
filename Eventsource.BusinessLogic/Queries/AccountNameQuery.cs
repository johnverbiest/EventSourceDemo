using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery
{
    public class AccountNameQuery: IQuery<AccountNameQuery.Result>
    {
        public int AccountNumber { get; set; }

        public class Result
        {
            public string AccountName { get; set; }
        }
    }
}