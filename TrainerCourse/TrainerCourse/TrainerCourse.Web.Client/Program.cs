using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TrainerCourse.Shared.Method;
using TrainerCourse.Shared.Services;
using TrainerCourse.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the TrainerCourse.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddTransient<NetworkAwareHttpClient>();
// Add HttpClient
builder.Services.AddHttpClient<ITrainerService, TrainerService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7285/");
});
builder.Services.AddHttpClient<ICourseService, CourseService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7285/");
});

await builder.Build().RunAsync();
