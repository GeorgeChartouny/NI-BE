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
builder.Services.AddTransient<LoaderService>();
builder.Services.AddTransient<AggregationService>();
builder.Services.AddTransient<GetDataService>();

// Serilog Configuration
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog(); //log on each request 

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
