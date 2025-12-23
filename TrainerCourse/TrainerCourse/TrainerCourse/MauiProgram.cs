using Microsoft.Extensions.Logging;
using TrainerCourse.Services;
using TrainerCourse.Shared.Method;
using TrainerCourse.Shared.Services;

namespace TrainerCourse
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
                });

            // Add device-specific services used by the TrainerCourse.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
            builder.Services.AddSingleton<ICameraService, CameraService>();
            builder.Services.AddTransient<NetworkAwareHttpClient>();
           
            builder.Services.AddHttpClient<ITrainerService, TrainerService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7285/");
            });
            builder.Services.AddHttpClient<ICourseService, CourseService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7285/");
            });
            

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
