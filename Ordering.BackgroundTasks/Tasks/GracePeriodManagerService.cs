using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.BackgroundTasks.Services;

namespace Ordering.BackgroundTasks.Tasks
{
    public class GracePeriodManagerService
         : BackgroundService
    {
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly BackgroundTaskSettings _settings;
        private readonly IOrderService _orderService;

        public GracePeriodManagerService(
            IOptions<BackgroundTaskSettings> settings,
            ILogger<GracePeriodManagerService> logger, 
            IOrderService orderService) {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _logger.LogDebug("GracePeriodManagerService is starting.");

            stoppingToken.Register(() => _logger.LogDebug("#1 GracePeriodManagerService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogDebug("GracePeriodManagerService background task is doing background work.");

                CheckConfirmedGracePeriodOrders();

                await Task.Delay(_settings.CheckUpdateTimeInSecond * 1000, stoppingToken);
            }

            _logger.LogDebug("GracePeriodManagerService background task is stopping.");

            await Task.CompletedTask;
        }

        private void CheckConfirmedGracePeriodOrders() {
            _logger.LogDebug("Checking confirmed grace period orders");

            var orderIds = GetConfirmedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                _orderService.SetOrderAwaitingValidation(orderId);
            }
        }

        private IEnumerable<int> GetConfirmedGracePeriodOrders() {
            IEnumerable<int> orderIds = new List<int>();

            using (var conn = new SqlConnection(_settings.ConnectionString)) {
                try {
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [orders] 
                            WHERE DATEDIFF(second, [CreatedDate], GETDATE()) >= @GracePeriodTime
                            AND [Status] = 0",
                        new { GracePeriodTime = _settings.GracePeriodTimeInSecond });
                } catch (SqlException exception) {
                    _logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}", exception.Message);
                }
            }

            return orderIds;
        }
    }
}