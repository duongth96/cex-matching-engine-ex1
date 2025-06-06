using CEX.MatchingEngine.Data;
using CEX.MatchingEngine.Demo.Configurations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddMemoryCache();


BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
var mongoConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseName = "MatchingEngineDb";

// Register MatchingEngineDbContext
builder.Services.AddSingleton(new MatchingEngineDbContext(mongoConnectionString, databaseName));

builder.Services.AddAppConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
