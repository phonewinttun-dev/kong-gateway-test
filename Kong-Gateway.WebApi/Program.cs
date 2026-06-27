using Kong_Gateway.WebApi.Data;
using Kong_Gateway.ConsoleApp.Data;
using Kong_Gateway.ConsoleApp.Service;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("BookDb"));

builder.Services.AddScoped<BookService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var bookService = scope.ServiceProvider.GetRequiredService<BookService>();

    await DbInitializer.Initialize(bookService);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
