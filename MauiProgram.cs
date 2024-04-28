using Kelimecim.DataAccess;
using Kelimecim.Utilities;
using Microsoft.Extensions.Logging;
/* Unmerged change from project 'Kelimecim (net7.0-maccatalyst)'
Before:
using Kelimecim.Utilities;
After:
using System.Reflection;
*/

/* Unmerged change from project 'Kelimecim (net7.0-android33.0)'
Before:
using Kelimecim.Utilities;
After:
using System.Reflection;
*/

/* Unmerged change from project 'Kelimecim (net7.0-windows10.0.19041.0)'
Before:
using Kelimecim.Utilities;
After:
using System.Reflection;
*/


namespace Kelimecim
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            InitializeDatabase(); // Veritabanını başlat

            builder.Services.AddDbContext<KelimecimDbContext>();
            builder.Services.AddTransient<SqliteProcess>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        //public static void VtSil()
        //{

        //    var context = new KelimecimDbContext();
        //    context.Dispose();

        //    var dbName = "KelimecimDb.db";
        //    var dbPath = PathDB.GetPath(dbName);
        //    int attempts = 0;
        //    while (attempts < 2 && File.Exists(dbPath))
        //    {
        //        try
        //        {
        //            File.Delete(dbPath);
        //            Console.WriteLine("Database successfully deleted.");
        //            break;
        //        }
        //        catch (IOException ex)
        //        {
        //            Console.WriteLine($"Failed to delete database file. Attempt {attempts + 1}: {ex.Message}");
        //            System.Threading.Thread.Sleep(1000);  // 1 saniye bekle
        //        }
        //        attempts++;
        //    }
        //}
        public static void InitializeDatabase()
        {
            var dbName = "KelimecimDb.db";
            var dbPath = PathDB.GetPath(dbName);
            Console.WriteLine($"Database path: {dbPath}");

            if (!File.Exists(dbPath))
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
                var resourceName = "Kelimecim.Resources.KelimecimDb.db";
                Stream stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    throw new FileNotFoundException($"Resource '{resourceName}' not found.");
                }

                using (var fileStream = new FileStream(dbPath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                    Console.WriteLine("Database copied from resources.");
                }
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }
        }

    }

}