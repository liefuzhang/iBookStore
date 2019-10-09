using System.Collections.Generic;
using System.Threading.Tasks;
using Membership.API.Infrastructure;
using Membership.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Membership.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly MembershipContext _membershipContext;

        public MembershipController(MembershipContext membershipContext)
        {
            _membershipContext = membershipContext;
        }

        // GET api/v1/[controller]/membershipTypes
        [HttpGet]
        [Route("membershipTypes")]
        public async Task<IEnumerable<MembershipType>> MembershipTypes()
        {
            return await _membershipContext.MembershipTypes.ToListAsync();
        }
    }
}
