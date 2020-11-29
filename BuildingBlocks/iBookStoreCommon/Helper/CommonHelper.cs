using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace iBookStoreCommon.Helper
{
    public static class CommonHelper
    {
        public static string GetConnectString(string connectionStringInConfig)
        {
            string connectionString;
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (string.IsNullOrEmpty(databaseUrl))
            {
                connectionString = connectionStringInConfig;
            }
            else
            {
                var databaseUri = new Uri(databaseUrl);
                var userInfo = databaseUri.UserInfo.Split(':');

                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = databaseUri.Host,
                    Port = databaseUri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = databaseUri.LocalPath.TrimStart('/')
                };
                connectionString = builder.ToString();
            }

            return connectionString;
        }
    }
}
