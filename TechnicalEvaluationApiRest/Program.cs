using Microsoft.EntityFrameworkCore;
using TechnicalEvaluationApiRest.Data;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Repositories;
using TechnicalEvaluationApiRest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IMunicipalityRepository, MunicipalityRepository>();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CountryService>();
builder.Services.AddTransient<DepartmentService>();
builder.Services.AddTransient<MunicipalityService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
