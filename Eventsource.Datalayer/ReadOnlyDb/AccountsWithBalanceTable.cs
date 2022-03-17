using System.Data;
using System.Data.SqlClient;
using Dapper;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.BusinessLogic.Events.FundsDeposited;
using Eventsource.BusinessLogic.Events.FundsTransfered;
using Eventsource.BusinessLogic.Events.FundsTransferedIn;
using Eventsource.BusinessLogic.Events.FundsWithdrawn;
using Eventsource.Datalayer.Appliers;
using JohnVerbiest.CQRS.Events;
using JohnVerbiest.CQRS.Queries;

namespace Eventsource.Datalayer.ReadOnlyDb;

public class AccountsWithBalanceTable: IReadOnlyModelHandler, IEventHandler<AccountCreatedEvent>, IEventHandler<FundsDepositedEvent>, IEventHandler<FundsWithdrawnEvent>, IEventHandler<FundsTransferedInEvent>, IEventHandler<FundsTransferedOutEvent>
{
    private readonly IEventStore _eventStore;

    public AccountsWithBalanceTable(IEventStore eventStore)
    {
        _eventStore = eventStore;

        if (!ReadOnlyDbSettings.EnableReadOnlyDb) return;

        BuildTable();
    }

    private static void BuildTable()
    {
        using var conn = new SqlConnection(ReadOnlyDbSettings.SqlConnectionString);
        conn.Open();
        BuildTableQueries(conn);
        conn.Close();
    }

    private static void BuildTableQueries(IDbConnection conn)
    {
        conn.Execute(TableCreateQuery);
    }

    private const string TableName = "AccountsWithBalance";
    private const string TableScheme = "dbo";
    private const string FullTableName = $"[{TableScheme}].[{TableName}]";
    private const string TableCreateQuery = @$"
IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = '{TableScheme}' 
                 AND  TABLE_NAME = '{TableName}'))
BEGIN
    CREATE TABLE {FullTableName}
    (
	    [AccountNumber] INT NOT NULL PRIMARY KEY, 
        [Name] VARCHAR(MAX) NULL,
        [Balance] DECIMAL (8, 2),
    )
END
";

    public async Task Rebuild()
    {
        if (ReadOnlyDbSettings.EnableReadOnlyDb == false) return;
        await using var conn = new SqlConnection(ReadOnlyDbSettings.SqlConnectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync($"DROP TABLE {FullTableName}");
        BuildTableQueries(conn);

        var events = await _eventStore.LoadEvents(new int[] { }, 
            typeof(AccountCreatedEvent),
            typeof(FundsDepositedEvent),
            typeof(FundsWithdrawnEvent),
            typeof(FundsTransferedOutEvent),
            typeof(FundsTransferedInEvent));

        foreach (var businessLogicEvent in events)
        {
            await DoTheThing(businessLogicEvent);
        }

        await conn.CloseAsync();
    }

    private async Task DoTheThing(IBusinessLogicEvent @event)
    {
        if (ReadOnlyDbSettings.EnableReadOnlyDb == false) return;


        await using var conn = new SqlConnection(ReadOnlyDbSettings.SqlConnectionString);
        await conn.OpenAsync();

        const string amountChangedQuery = $"UPDATE {FullTableName} SET [Balance] = [Balance] + @change WHERE [AccountNumber] = @accountNumber";

        switch (@event)
        {
            case AccountCreatedEvent e:
                var account = e.Apply();
                const string accountQuery = $"INSERT INTO {FullTableName} ([AccountNumber], [Name], [Balance]) VALUES (@accountNumber, @name, 0)";
                await conn.ExecuteAsync(accountQuery, new { accountnumber = account.accountNumber, name = account.accountName });
                break;
            case FundsDepositedEvent e:
                var changeDeposit = e.Apply(0);
                await conn.ExecuteAsync(amountChangedQuery, new { change = changeDeposit, accountNumber = e.AccountNumber });
                break;
            case FundsWithdrawnEvent e:
                var changeWithdrawn = e.Apply(0);
                await conn.ExecuteAsync(amountChangedQuery, new { change = changeWithdrawn, accountNumber = e.AccountNumber });
                break;
            case FundsTransferedInEvent e:
                var changeTransferIn = e.Apply(0);
                await conn.ExecuteAsync(amountChangedQuery, new { change = changeTransferIn, accountNumber = e.AccountNumber });
                break;
            case FundsTransferedOutEvent e:
                var changeTranferOut = e.Apply(0);
                await conn.ExecuteAsync(amountChangedQuery, new { change = changeTranferOut, accountNumber = e.AccountNumber });
                break;
        }

        await conn.CloseAsync();
    }

    public Task Handle(AccountCreatedEvent @event)
    {
        return DoTheThing(@event);
    }

    public Task Handle(FundsDepositedEvent @event)
    {
        return DoTheThing(@event);
    }

    public Task Handle(FundsWithdrawnEvent @event)
    {
        return DoTheThing(@event);
    }

    public Task Handle(FundsTransferedInEvent @event)
    {
        return DoTheThing(@event);
    }

    public Task Handle(FundsTransferedOutEvent @event)
    {
        return DoTheThing(@event);
    }
}