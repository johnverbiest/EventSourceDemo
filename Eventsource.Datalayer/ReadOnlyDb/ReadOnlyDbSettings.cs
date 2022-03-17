namespace Eventsource.Datalayer.ReadOnlyDb;

public static class ReadOnlyDbSettings
{
    public static bool EnableReadOnlyDb => true;

    public static string SqlConnectionString =>
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EventSourceDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
}