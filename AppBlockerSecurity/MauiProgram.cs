using AppBlockerSecurity.Services;
using AppBlockerSecurity.ViewModels;
using AppBlockerSecurity.Views;
using Microsoft.Extensions.Logging;

namespace AppBlockerSecurity
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MathPuzzleService>();

            builder.Services.AddTransient<LockScreenViewModel>();
            builder.Services.AddTransient<LockScreenPage>();

            return builder.Build();
        }
    }
}
