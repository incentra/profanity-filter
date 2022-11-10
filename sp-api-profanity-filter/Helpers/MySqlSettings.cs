using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SP.Profanity.Interfaces;

namespace SP.Profanity.Helpers
{
    public class MySqlSettings : IMySqlSettings
    {
        private readonly string connectionString;

        public MySqlSettings(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("filters");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

    }

}