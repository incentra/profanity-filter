using SP.Profanity.Interfaces;

namespace SP.Profanity.Helpers
{
    public class AppSettings : IAppSettings
    {
        public string MySqlConnectionString { get; private set; }
        public string ApiToken { get; private set; }
        public AppSettings()
        {
            MySqlConnectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            ApiToken = System.Environment.GetEnvironmentVariable("API_TOKEN");
        }
    }
}