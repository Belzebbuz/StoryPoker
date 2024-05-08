
using StoryPoker.Server.Silo.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddOrleans();
var app = builder.Build();
app.Run();
