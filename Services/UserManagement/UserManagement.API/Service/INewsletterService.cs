using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagement.API.Service
{
    public interface INewsletterService
    {
        void SignUpNewsletter(string email);
    }
}
