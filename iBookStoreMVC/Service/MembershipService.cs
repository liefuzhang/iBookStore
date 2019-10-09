using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using iBookStoreMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace iBookStoreMVC.Service
{
    public class MembershipService : IMembershipService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MembershipService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public MembershipService(HttpClient httpClient, ILogger<MembershipService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.MembershipUrl}/api/v1/membership/";
        }

        public async Task<IEnumerable<MembershipType>> GetMembershipTypes()
        {
            var url = API.Membership.GetMembershipTypes(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(url);

            var membershipTypes = JsonConvert.DeserializeObject<IEnumerable<MembershipType>>(responseString);

            return membershipTypes;
        }
    }
}