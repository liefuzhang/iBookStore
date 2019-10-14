namespace iBookStoreMVC
{
    public class AppSettings
    {
        public string CatalogUrl { get; set; } 
        public string IdentityUrl { get; set; } 
        public string CallbackUrl { get; set; }
        public object BasketUrl { get; internal set; }
    }
}