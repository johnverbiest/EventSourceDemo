namespace Eventsource.Datalayer.ReadOnlyDb;

public interface IReadOnlyModelHandler
{
    Task Rebuild();
}