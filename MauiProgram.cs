using Microsoft.Extensions.Logging;
using Kelimecim.DataAccess;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Kelimecim.Utilities;

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
        public static void InitializeDatabase()
        {
            var dbName = "KelimecimDb.db";
            var dbPath = PathDB.GetPath(dbName);
            Console.WriteLine($"Database path: {dbPath}");

            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentException("Database path cannot be null or empty.", nameof(dbPath));
            }

            if (!File.Exists(dbPath))
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
                var resourceName = "Kelimecim.Resources.KelimecimDb.db"; // Bu ismi projenize göre düzenleyin
                Stream stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    throw new FileNotFoundException($"Resource '{resourceName}' not found.");
                }

                using (var fileStream = new FileStream(dbPath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

    }

}