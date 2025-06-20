using AutoScheduler.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using AutoScheduler.Domain.Interfaces.Service;
using AutoScheduler.Application.Services;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.DataAccess.Repositories;
using AutoScheduler.Application.Mappers.AutoMapper;
using Microsoft.AspNetCore.Identity;
using AutoScheduler.Domain.Entities.Users;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors((options) =>
{
	options.AddDefaultPolicy((policy) =>
	{
		policy.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowAnyOrigin();
	});
});

var connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<SchedulerContext>(options=>{
	options.UseSqlServer(connectionString);
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication().AddJwtBearer(jwtOptions => {
	jwtOptions.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddIdentityApiEndpoints<User>()
	.AddEntityFrameworkStores<SchedulerContext>();


builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Services
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJWTService, JWTService>();

//Repositories
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<ITimesheetRepository, TimesheetRepository>();

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
app.UseAuthentication();

app.UseCors();

app.MapControllers();

app.MapIdentityApi<User>();

app.Run();
