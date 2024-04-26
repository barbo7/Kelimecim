using Microsoft.Extensions.Logging;
using Kelimecim.DataAccess;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            builder.Services.AddDbContext<KelimecimDbContext>();
            builder.Services.AddTransient<SqliteProcess>();

            var dbContext = new KelimecimDbContext();
            dbContext.Database.EnsureCreated();
            dbContext.Dispose();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}