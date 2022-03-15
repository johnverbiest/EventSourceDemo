using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery
{
    public class AccountBalanceQuery: IQuery<AccountBalanceQuery.Result>
    {
        public int AccountNumber { get; set; }
        
        public class Result
        {
            public decimal Balance { get; set; }
        }
    }
}