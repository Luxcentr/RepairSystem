using RepairApp.Repositories;
using RepairApp.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "repairs.json");
builder.Services.AddSingleton<IRepairRepository>(new JsonRepairRepository(dataPath));
builder.Services.AddScoped<IRepairService, RepairService>();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Run();