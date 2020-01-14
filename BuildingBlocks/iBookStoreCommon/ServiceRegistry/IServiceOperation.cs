namespace iBookStoreCommon.ServiceRegistry
{
    public interface IServiceOperation
    {
        string HttpMethod { get; }

        string Path { get; }
    }
}
