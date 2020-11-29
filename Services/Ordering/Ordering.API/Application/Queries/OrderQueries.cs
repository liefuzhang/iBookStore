using Dapper;
using Ordering.API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Ordering.API.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private string _connectionString = string.Empty;

        public OrderQueries(string connectionString) {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersForUserAsync(string userId) {
            using (var connection = new NpgsqlConnection(_connectionString)) {
                connection.Open();

                return await connection.QueryAsync<OrderSummary>($@"SELECT o.""Id"" as OrderNumber,o.""CreatedDate"" as CreatedDate,o.""Status"" as Status, SUM(oi.""Units""*ROUND(oi.""UnitPrice""*o.""CurrencyRate"", 2)) as Total, o.""Currency"" as Currency
                     FROM ""Orders"" o
                     LEFT JOIN ""OrderItems"" oi ON  o.""Id"" = oi.""OrderId""                   
                     LEFT JOIN ""Buyers"" ob on o.""BuyerId"" = ob.""Id""
                     WHERE ob.""IdentityGuid"" = '{userId}'
                     GROUP BY o.""Id"", o.""CreatedDate"", o.""Status"", o.""Currency""
                     ORDER BY o.""Id""");
            }
        }
    }
}
