using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace iBookStoreMVC.Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserManagementService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public UserManagementService(HttpClient httpClient, ILogger<UserManagementService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.ApiGatewayUrl}/api/userManagement";
        }

        public async Task SignUpNewsletter(string email)
        {
            var url = API.UserManagement.SignUpNewsletter(_remoteServiceBaseUrl);
            var content = new StringContent(JsonConvert.SerializeObject(new { email }), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }
    }
}