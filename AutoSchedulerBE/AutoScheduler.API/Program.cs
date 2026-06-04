using AutoScheduler.Application.Mappers.AutoMapper;
using AutoScheduler.Application.Services;
using AutoScheduler.DataAccess;
using AutoScheduler.DataAccess.Repositories;
using AutoScheduler.DataAccess.Seeders;
using AutoScheduler.Domain.Entities.Users;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors((options) =>
{
	options.AddPolicy("CorsPolicy", (policy) =>
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

builder.Services.AddIdentityApiEndpoints<User>()
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<SchedulerContext>();

builder.Services.AddAuthentication(options=>
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions => {
		jwtOptions.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = builder.Configuration["JWT:Issuer"],
			ValidateAudience = true,
			ValidAudience = builder.Configuration["JWT:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
		};
});

builder.Services.AddAuthorization();


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
builder.Services.AddSwaggerGen(o =>
	{
		o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "JWT auth header using Bearer scheme",
				Type = SecuritySchemeType.Http,

        });
		o.AddSecurityRequirement(new OpenApiSecurityRequirement {
				{
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference {
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					new string[] {}
				}
			});
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

//seed dev testing data
if (app.Environment.IsDevelopment())
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<SchedulerContext>();
		await dbContext.Database.MigrateAsync();

		//seed roles
		await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
		//seed test users with each role
		await IdentitySeeder.SeedTestUsersAsync(scope.ServiceProvider);

		await DataSeeder.SeedFromCsvAsync(scope.ServiceProvider);

	}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
