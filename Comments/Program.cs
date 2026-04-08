using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var config = builder.Configuration;

// KEEP other services
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();



// HTTPS redirect
if (!env.IsDevelopment())
{
	builder.Services.AddHttpsRedirection(options =>
	{
		options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
		options.HttpsPort = 443;
	});
}

// Forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
	options.KnownProxies.Clear();
});


var app = builder.Build();

// Static files
app.UseStaticFiles();


// Security header
app.Use(async (context, next) =>
{
	context.Response.OnStarting(() =>
	{
		context.Response.Headers.Add("Permissions-Policy", "interest-cohort=()");
		return Task.CompletedTask;
	});

	await next();
});


// Routing
app.UseRouting();

// Forwarded headers
app.UseForwardedHeaders();

// app.UseAuthentication();
// app.UseAuthorization();


// HTTPS redirect
if (!env.IsDevelopment())
{
	app.UseHttpsRedirection();
}


app.MapGet("/", () => "OK");
app.MapGet("/consultations", () => "OK");


app.Run();
