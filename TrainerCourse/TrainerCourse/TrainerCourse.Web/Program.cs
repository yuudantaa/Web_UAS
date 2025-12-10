using TrainerCourse.Shared.Method;
using TrainerCourse.Shared.Services;
using TrainerCourse.Web.Components;
using TrainerCourse.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the TrainerCourse.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddHttpClient<ITrainerService, TrainerService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7285/");
});
builder.Services.AddHttpClient<ICourseService, CourseService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7285/");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(TrainerCourse.Shared._Imports).Assembly,
        typeof(TrainerCourse.Web.Client._Imports).Assembly);

app.Run();
