using EventManagementSystem.Repositories;
using EventManagementSystem.Services;
using EventManagementSystem.Utilities;
using System.Data;
using EventManagementSystem.Data;
using EventManagementSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using EventManagementSystem;
using Quartz;





var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSignalR();
builder.Services.AddSingleton(new DatabaseConnection(connectionString));
builder.Services.AddTransient<IDbConnection>(sp =>
    sp.GetRequiredService<DatabaseConnection>().CreateConnection());

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddSingleton<IJob, DynamicPricingJob>();
builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(DynamicPricingJob),
    cronExpression: "0 */20 * * * ?"));

builder.Services.AddSingleton<IHubContext<ReservationHub>>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IPriceRepository, PriceRepository>();
builder.Services.AddScoped<IPriceService, PriceService>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapHub<ReservationHub>("/reservationHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
