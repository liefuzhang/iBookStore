using System.Threading.Tasks;

namespace iBookStoreMVC.Service
{
    public interface IUserManagementService
    {
        Task SignUpNewsletter(string email);
    }
}