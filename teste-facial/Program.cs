using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using teste_facial.Services;

namespace teste_facial
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        })
                        .ConfigureServices((context, services) =>
                        {
                            services.Configure<DatabaseSettings>(context.Configuration.GetSection("ConnectionStrings"));

                            // Registrar serviços
                            services.AddSingleton<IDatabaseService, DatabaseService>();
                            services.AddTransient<SalvarDadosFoto>();
                            services.AddSingleton<frmCapturaFoto>();
                        })
                        .Build();

            ApplicationConfiguration.Initialize();
            var form = host.Services.GetRequiredService<frmCapturaFoto>();
            Application.Run(form);

        }
    }
}