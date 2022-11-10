namespace SP.Profanity.Interfaces
{
    public interface IAppSettings
    {
        string MySqlConnectionString { get; }
        string ApiToken { get; }
    }
}