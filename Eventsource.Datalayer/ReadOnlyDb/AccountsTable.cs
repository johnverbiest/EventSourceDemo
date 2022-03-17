using System.Data;
using System.Data.SqlClient;
using Dapper;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Eventsource.BusinessLogic.Events.AccountCreated;
using Eventsource.Datalayer.Appliers;
using JohnVerbiest.CQRS.Events;

namespace Eventsource.Datalayer.ReadOnlyDb;

public class AccountsTable: IReadOnlyModelHandler, IEventHandler<AccountCreatedEvent>
{
    private readonly IEventStore _eventStore;
    private const string TableName = "Accounts";
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
        [Name] VARCHAR(MAX) NULL
    )
END
";

    public AccountsTable(IEventStore eventStore)
    {
        _eventStore = eventStore;
        if (!ReadOnlyDbSettings.EnableReadOnlyDb) return;

        BuildTable();
    }

    public Task Handle(AccountCreatedEvent @event)
    {
        return DoTheThing(@event);
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


    public async Task Rebuild()
    {
        if (ReadOnlyDbSettings.EnableReadOnlyDb == false) return;
        await using var conn = new SqlConnection(ReadOnlyDbSettings.SqlConnectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync($"DROP TABLE {FullTableName}");
        BuildTableQueries(conn);

        var events = await _eventStore.LoadEvents(new int[] { }, typeof(AccountCreatedEvent));

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

        switch (@event)
        {
            case AccountCreatedEvent e:
                var account = e.Apply();
                const string query = $"INSERT INTO {FullTableName} ([AccountNumber], [Name]) VALUES (@accountNumber, @name)";
                await conn.ExecuteAsync(query, new { accountnumber = account.accountNumber, name = account.accountName });
                break;
        }

        await conn.CloseAsync();
    }
}