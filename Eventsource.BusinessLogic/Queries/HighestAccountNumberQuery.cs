using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery
{
    public class HighestAccountNumberQuery: IQuery<HighestAccountNumberQuery.Result>
    {
        public class Result
        {
            public int HighestAccountNumber { get; set; }
        }
    }
}