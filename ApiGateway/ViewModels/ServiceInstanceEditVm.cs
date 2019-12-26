namespace ApiGateway.ViewModels
{
    public class ServiceInstanceEditVm
    {
        public string Scheme { get; set; }

        public string IpAddress { get; set; }

        public string Port { get; set; }

        public bool? IsStatic { get; set; }
    }
}