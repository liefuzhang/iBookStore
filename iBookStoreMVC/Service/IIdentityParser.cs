using System.Security.Principal;

namespace iBookStoreMVC.Service
{
    public interface IIdentityParser<T>
    {
        T Parse(IPrincipal principal);
    }
}