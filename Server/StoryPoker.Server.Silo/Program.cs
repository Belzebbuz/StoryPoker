
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.Services.Factories;
using StoryPoker.Server.Silo.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddOrleans();
builder.Services.AddSingleton<IRoomStateResponseFactory, InternalRoomResponseFactory>();
var app = builder.Build();
app.Run();
