using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TrainerCourse.Backend.Data;
using TrainerCourse.Backend.DataEF;
using TrainerCourse.Backend.DbMapper;
using TrainerCourse.Backend.DTO;
using TrainerCourse.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICourse, CourseEF>();
builder.Services.AddScoped<ITrainer, TrainerEF>();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")), // <-- Ensure this path is correct
    RequestPath = "/uploads"
});

app.UseAuthorization();

app.UseAntiforgery();


app.MapGet("/Courses", (ICourse courseData, IMapper mapper) =>
{
    var courses = courseData.GetCourses();
    var courseDTOs = mapper.Map<List<CourseDTO>>(courses);
    return Results.Ok(courseDTOs);
});

app.MapGet("/Courses/{id}", (ICourse courseData, IMapper mapper, int id) =>
{
    var course = courseData.GetCourseById(id);
    if (course == null)
    {
        return Results.NotFound();
    }
    var courseDTO = mapper.Map<CourseDTO>(course);
    return Results.Ok(courseDTO);
});

app.MapPost("/Courses", (ICourse courseData, CourseAddDTO courseAddDTO, IMapper mapper) => {
    var course = mapper.Map<Course>(courseAddDTO);

    try
    {
        var newCourse = courseData.AddCourse(course);
        var courseDTO = mapper.Map<CourseDTO>(newCourse);
        return Results.Created($"/Courses/{newCourse.CourseId}", courseDTO);
    }
    catch (DbUpdateException dbex)
    {
        return Results.Problem("Database error occurred");
    }
    catch (System.Exception ex)
    {
        return Results.Problem("An error occurred");
    }
});

app.MapPut("/Courses", (ICourse courseData, Course course) => {
    var updatedCourse = courseData.UpdateCourse(course);
    return Results.Ok(updatedCourse);
});

app.MapDelete("/Courses/{id}", (ICourse courseData, int id) => {
    try
    {
        courseData.DeleteCourse(id);
        return Results.Ok(); 
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/Courses/upload", async (ICourse courseData, IFormFile file) => {
    try
    {
        var fileName = courseData.uploadImage(file);
        return Results.Ok(new
        {
            success = true,
            fileName = fileName,
            imageUrl = $"/uploads/{fileName}"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = ex.Message
        });
    }
}).DisableAntiforgery();

app.MapGet("/Courses/search/{searchTerm}", (ICourse courseData, IMapper mapper, string searchTerm) =>
{
    var courses = courseData.SearchCourses(searchTerm);
    var courseDTOs = mapper.Map<List<CourseDTO>>(courses);
    return Results.Ok(courseDTOs);
});

app.MapGet("/Trainers", (ITrainer TrainerData, IMapper mapper) => {
    var trainers = TrainerData.GetTrainer();
    var trainerDTOs = mapper.Map<List<TrainerDTO>>(trainers);
    return Results.Ok(trainerDTOs);
});

app.MapGet("/Trainers/{id}", (ITrainer TrainerData, int id, IMapper mapper) =>
{
    var trainer = TrainerData.GetTrainerById(id);
    if (trainer == null)
    {
        return Results.NotFound();
    }
    var trainerDTO = mapper.Map<TrainerDTO>(trainer);
    return Results.Ok(trainerDTO);
});

app.MapPost("/Trainers", (ITrainer TrainerData, TrainerAddDTO trainerAddDTO, IMapper mapper) => {

    var trainer = mapper.Map<Trainer>(trainerAddDTO);

    try
    {
        var newTrainer = TrainerData.AddTrainer(trainer);
        var trainerDTOs = mapper.Map<TrainerDTO>(newTrainer);
        return Results.Created($"/Trainers/{newTrainer.TrainerId}", trainerDTOs);
    }
    catch (DbUpdateException dbex)
    {
        return Results.Problem("Database error occurred");
    }
    catch (System.Exception ex)
    {
        return Results.Problem("An error occurred");
    }
});

app.MapPut("/Trainers", (ITrainer trainerData, Trainer trainer) => {
    var putTrainer = trainerData.UpdateTrainer(trainer);
    return putTrainer;
});

app.MapDelete("/Trainers/{id}", (ITrainer TrainerData, int id) =>
{
    try
    {
        TrainerData.DeleteTrainer(id);
        return Results.Ok(); 
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("masih memiliki course terkait"))
    {
        return Results.Conflict(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/Trainers/search/{searchTerm}", (ITrainer trainerData, IMapper mapper, string searchTerm) =>
{
    var trainers = trainerData.SearchTrainers(searchTerm);
    var trainerDTOs = mapper.Map<List<TrainerDTO>>(trainers);
    return Results.Ok(trainerDTOs);
});


app.Run();
