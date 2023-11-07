using System.Data;
using babyNI_BE.Watcher;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
using NI_BE.DataDb;
//using Vertica.Data.VerticaClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHostedService<Worker>();

// DB Connection
//builder.Services.AddDbContext<AppDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var dbConnection = new DBConnection();
dbConnection.EstablishConnection();

//using IDbConnection dbConnection = new SqlConnection("DefaultConnection");


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
