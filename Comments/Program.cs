var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/consultations", () => "OK");

app.MapGet("/", () => "OK");

app.Run();
