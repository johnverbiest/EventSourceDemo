using System;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery
{
    public class AccountHistoryQuery: IQuery<AccountHistoryQuery.Result>
    {
        public int AccountNumber { get; set; }

        public class Result
        {
            public HistoryObject[] Events { get; set; }
            public class HistoryObject
            {
                public DateTime date { get; set; }
                public string Description { get; set; }
                public decimal Balance { get; set; }
            }
            
        }
    }
}