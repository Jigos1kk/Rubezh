using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    context.Request.Headers["X-Rubezh-Gateway"] = "v1.0";
    Console.WriteLine($"[Rubezh] -> {context.Request.Method} {context.Request.Path} от {context.Connection.RemoteIpAddress}");

    await next(context); 
    
    stopwatch.Stop();
    Console.WriteLine($"[Rubezh] <- {context.Response.StatusCode} | Время: {stopwatch.ElapsedMilliseconds}ms");
});

app.MapReverseProxy();

app.Run();
