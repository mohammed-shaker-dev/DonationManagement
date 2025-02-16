using Dashboard.Api;
using Dashboard.Infrastructure.Data;
using Dashboard.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(c =>
    c.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var baseUrlConfig = new BaseUrlConfiguration();
builder.Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddInfrastructureDependencies(isDevelopment);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await app.SeedDatabaseAsync();
app.UseCors("AllowAll");
app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
