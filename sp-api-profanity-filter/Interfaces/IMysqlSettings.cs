using MySql.Data.MySqlClient;

namespace SP.Profanity.Interfaces
{
    public interface IMySqlSettings
    {
        MySqlConnection GetConnection();
    }
}
