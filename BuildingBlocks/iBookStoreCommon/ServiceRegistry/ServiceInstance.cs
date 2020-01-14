namespace iBookStoreCommon.ServiceRegistry
{
    public class ServiceInstance
    {
        public int? ServiceInstanceId { get; set; }
        public string ServiceName { get; set; }
        public string Scheme { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}
