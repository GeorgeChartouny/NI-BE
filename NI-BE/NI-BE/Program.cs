using System.Data;
using babyNI_BE.Watcher;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NI_BE.DataDb;
using NI_BE.Services;
using Serilog;
using Vertica.Data.VerticaClient;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                     .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<ParserService>();
//builder.Services.AddTransient<GetDataService>();

// Serilog Configuration
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog(); //log on each request 
string watcherFolder = Environment.GetEnvironmentVariable("watcherFolder");
string parserFolder = Environment.GetEnvironmentVariable("parserFolder");
string loadedData = Environment.GetEnvironmentVariable("loadedData");
string oldDataFolder = Environment.GetEnvironmentVariable("oldDataFolder");
string unloadedData = Environment.GetEnvironmentVariable("unloadedData");


if (!Directory.Exists(watcherFolder))
{
    Directory.CreateDirectory(watcherFolder);
    Log.Information("watcher folder created");
}

if (!Directory.Exists(parserFolder))
{
    Directory.CreateDirectory(parserFolder);
    Log.Information("parsing folder created");
}

if (!Directory.Exists(loadedData))
{
    Directory.CreateDirectory(loadedData);
    Log.Information("loader folder created");
}

if (!Directory.Exists(oldDataFolder))
{
    Directory.CreateDirectory(oldDataFolder);
    Log.Information("oldData folder created");
}

if (!Directory.Exists(unloadedData))
{
    Directory.CreateDirectory(unloadedData);
    Log.Information("unloaded folder created");
}

// DB Connection
//builder.Services.AddDbContext<AppDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var dbConnection = new DBConnection();
dbConnection.EstablishConnection();

//using IDbConnection dbConnection = new SqlConnection("DefaultConnection");


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
