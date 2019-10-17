using Microsoft.AspNetCore.Identity;

namespace iBookStoreMVC.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CardHolderName { get; set; }
        public int CardType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}