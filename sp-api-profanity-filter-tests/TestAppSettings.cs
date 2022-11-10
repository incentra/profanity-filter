using SP.Profanity.Interfaces;

namespace SP.Profanity.Tests
{
    public class TestAppSettings : IAppSettings
    {
        public string MySqlConnectionString { get; private set; }
        public string ApiToken { get; }
        public TestAppSettings()
        {
            MySqlConnectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        }

    }
}