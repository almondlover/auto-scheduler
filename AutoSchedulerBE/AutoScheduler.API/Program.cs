using AutoScheduler.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using AutoScheduler.Domain.Interfaces.Service;
using AutoScheduler.Application.Services;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.DataAccess.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy((policy) =>
    {
        policy.AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<SchedulerContext>(options=>{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddScoped<IGroupRepository, GroupRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
