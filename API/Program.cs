using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
// Add authentication services to the dependency injection container with the JwtBearer authentication scheme.
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
  .WithOrigins("https://localhost:4200"));

app.UseAuthentication(); // do you have a valid token?
app.UseAuthorization(); // you have, so what you can you do?

app.MapControllers();

app.Run();
