using Microsoft.Extensions.Options;

namespace teste_facial.Services
{
    public interface IDatabaseService
    {
        string GetConnectionString();
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly DatabaseSettings _settings;

        public DatabaseService(IOptions<DatabaseSettings> options)
        {
            _settings = options.Value;
        }

        public string GetConnectionString()
        {
            return _settings.MinhaConexao;
        }
    }

}
