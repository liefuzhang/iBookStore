using Dapper;
using Ordering.API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private string _connectionString = string.Empty;

        public OrderQueries(string connectionString) {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersForUserAsync(string userId) {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                return await connection.QueryAsync<OrderSummary>(@"SELECT o.[Id] as OrderNumber,o.[CreatedDate] as [CreatedDate],o.[Status] as [Status], SUM(oi.units*oi.unitprice) as Total
                     FROM [dbo].[Orders] o
                     LEFT JOIN[dbo].[orderitems] oi ON  o.Id = oi.orderid                   
                     LEFT JOIN[dbo].[buyers] ob on o.BuyerId = ob.Id
                     WHERE ob.IdentityGuid = @userId
                     GROUP BY o.[Id], o.[CreatedDate], o.[Status]
                     ORDER BY o.[Id]", new { userId });
            }
        }
    }
}
