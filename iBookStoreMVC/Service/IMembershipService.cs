using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipType>> GetMembershipTypes();
    }
}