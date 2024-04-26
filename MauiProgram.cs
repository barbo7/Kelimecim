using Microsoft.Extensions.Logging;
using Kelimecim.DataAccess;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

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
            var dbPath = Kelimecim.Utilities.PathDB.GetPath(dbName);
            if (!File.Exists(dbPath))
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
                Stream stream = assembly.GetManifestResourceStream($"Kelimecim.Resources.{dbName}");
                using (var fileStream = new FileStream(dbPath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }

}