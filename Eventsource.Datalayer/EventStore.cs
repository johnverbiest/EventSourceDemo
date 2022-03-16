using System.Collections.Concurrent;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Eventsource.BusinessLogic.Dependencies;
using Eventsource.BusinessLogic.Events;
using Microsoft.Extensions.Caching.Memory;

namespace Eventsource.Datalayer;

public class EventStore: IEventStore
{
    private readonly TableServiceClient _serviceClient = new TableServiceClient("UseDevelopmentStorage=true");
    private const string TablePrefix = "EventSource";
    private readonly ConcurrentDictionary<Type, TableClient> _tables = new ConcurrentDictionary<Type, TableClient>();



    public Task SaveEvent<T>(T @event) where T : IBusinessLogicEvent
    {
        var table = GetTableClient(@event.GetType());
        var entity = Entity.Serialize(@event);
        return table.AddEntityAsync(entity);
    }

    private TableClient GetTableClient(Type etype) 
    {
        var table = _tables.GetOrAdd(etype, type =>
        {
            var tableName = $"{TablePrefix}{type.Name}";
            _serviceClient.CreateTableIfNotExists(tableName);
            return _serviceClient.GetTableClient(tableName);
        });
        return table;
    }

    private readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
    public async Task<IBusinessLogicEvent[]> LoadEvents(int[] accounts, params Type[] eventTypes)
    {
        // Create the filter
        var filter = string.Join(" or ", accounts.Select(e => $"(PartitionKey eq '{e}')"));
        if (filter != "") filter = $"({filter}) and ";
        var result = new List<IBusinessLogicEvent>();


        foreach (var eventType in eventTypes)
        {
            var table = GetTableClient(eventType);
            var cacheKey = $"{table.Name}-{filter}";
            var loadedEvents = await cache.GetOrCreateAsync(cacheKey, entry => Task.FromResult(new HashSet<Entity>()));
            var newestEvent = loadedEvents.Any() ? loadedEvents.Max(x => x.Timestamp) : DateTimeOffset.MinValue;

            var entities = table.Query<Entity>(filter: $"{filter}(Timestamp gt datetime'{newestEvent:yyyy-MM-ddTHH:mm:ssZ}')");
            foreach (var entity in entities)
            {
                if (loadedEvents.All(x => x.GetHashCode() != entity.GetHashCode())) loadedEvents.Add(entity);
            }

            var resultSet =
                loadedEvents.Select(e => (IBusinessLogicEvent)JsonSerializer.Deserialize(e.RawData, eventType, new JsonSerializerOptions()));

            cache.Set(cacheKey, loadedEvents, TimeSpan.FromMinutes(1));

            result.AddRange(resultSet);
        }


        return result.OrderBy(x => x.EventRaised).ToArray();
    }

    
    class Entity: ITableEntity
    {
        public static Entity Serialize<T>(T @event) where T : IBusinessLogicEvent
        {
            var result = new Entity()
            {
                PartitionKey = @event.AccountNumber.ToString(),
                RowKey = @event.EventId.ToString(),
                Timestamp = @event.EventRaised,
                Raised = @event.EventRaised,
                AccountNumber = @event.AccountNumber,
                FullType = @event.GetType().FullName,
                RawData = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions())
            };
            return result;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public DateTime Raised { get; set; }
        public int AccountNumber { get; set; }
        public string FullType { get; set; }
        public string RawData { get; set; }

        public override int GetHashCode()
        {
            return $"{FullType}|{PartitionKey}|{RowKey}".GetHashCode();
        }
    }


}