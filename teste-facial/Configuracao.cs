using Microsoft.Extensions.Configuration;


namespace teste_facial
{
    public static class Configuracao
    {
        private static IConfigurationRoot configuration;

        static Configuracao()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // ou Application.StartupPath
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        public static string ConnectionString => configuration.GetConnectionString("MinhaConexao");
    }


}
