using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace PSInventory
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        public static string UserName { get; set; } =  "Default User";
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Run the main form
            Application.Run(new Menu());
        }

        //private static void ConfigureServices(IServiceCollection services)
        //{
        //    // Registrar el DbContext (usa tu conexión LocalDB aquí)
        //    services.AddDbContext<InventarioContext>(options =>
        //        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PSInventoryDB;Trusted_Connection=True;"));

        //    // Registrar tus formularios y servicios
        //    services.AddScoped<Login>();
        //    // Agrega otros formularios o servicios aquí
        //}
    }
}