using MongoDB.Driver;
using NLog;
using NLog.Web;
using OnlineService.Repositories;
using OnlineService.Repositories.Interfaces;

var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

logger.Debug("Starting OnlineService");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString =
        Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")
        ?? "mongodb://admin:secret123@localhost:27017/?authSource=admin";

    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();

    return mongoClient.GetDatabase("OnlineServiceDb");
});

builder.Services.AddScoped<IVideoRepository, VideoRepositoryMongoDb>();
builder.Services.AddScoped<ITrainingProgramRepository, TrainingProgramRepositoryMongoDb>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();