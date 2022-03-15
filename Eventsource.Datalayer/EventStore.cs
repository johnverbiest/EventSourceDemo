using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;

namespace Eventsource.Datalayer;

public class EventStore: IEventStore
{
    private readonly TableServiceClient _serviceClient = new TableServiceClient("UseDevelopmentStorage=true");
    private const string TableName = "EventSource";
    private readonly TableClient _table;

    public EventStore()
    {
        _serviceClient.CreateTableIfNotExists(TableName);
        _table = _serviceClient.GetTableClient(TableName);
    }

    public Task SaveEvent<T>(T @event) where T : IBusinessLogicEvent
    {
        var entity = Entity.Serialize(@event);
        return _table.AddEntityAsync(entity);
    }

    public Task<IBusinessLogicEvent[]> LoadEvents(params Type[] eventTypes)
    {
        // Create the filter
        var filter = string.Join(" or ", eventTypes.Select(e => $"(PartitionKey eq '{e.FullName}')"));
        
        // Create the types dictionary for translations
        var types = eventTypes.ToDictionary(x => x.FullName, x => x);

        // Getting the entities and converting them to their classes
        var entities = _table.Query<Entity>(filter: filter);
        var resultSet =
            entities.Select(e => (IBusinessLogicEvent)JsonSerializer.Deserialize(e.RawData, types[e.PartitionKey] ?? typeof(AbstractBusinessLogicEvent), new JsonSerializerOptions()));

        return Task.FromResult(resultSet.OrderBy(x => x.EventRaised).ToArray());
    }


    class Entity: ITableEntity
    {
        public static Entity Serialize<T>(T @event) where T : IBusinessLogicEvent
        {
            var result = new Entity()
            {
                PartitionKey = @event.GetType().FullName,
                RowKey = @event.EventId.ToString(),
                Timestamp = @event.EventRaised,
                Raised = @event.EventRaised,
                RawData = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions())
            };
            return result;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public DateTime Raised { get; set; }
        public string RawData { get; set; }
    }


}