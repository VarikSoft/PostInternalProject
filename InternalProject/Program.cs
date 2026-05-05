using Microsoft.EntityFrameworkCore;
using InternalProject.Application;
using InternalProject.Data;
using InternalProject.Infrastructure;
using InternalProject.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<EfPostRepository>();
builder.Services.AddScoped<IPostReadRepository>(sp => sp.GetRequiredService<EfPostRepository>());
builder.Services.AddScoped<IPostWriteRepository>(sp => sp.GetRequiredService<EfPostRepository>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
