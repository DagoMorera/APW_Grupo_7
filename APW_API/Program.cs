using Microsoft.EntityFrameworkCore;
using APW.Business;
using APW.Data.MSSQLEF;
using APW.Architecture.Providers;
using APW.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Servicios base de la Api
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexion a la base de datos APW
builder.Services.AddDbContext<ApwDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - Roles
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleBusiness, RoleBusiness>();

// Dependency Injection - Users
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();

// Dependency Injection - Sources
builder.Services.AddScoped<ISourceRepository, SourceRepository>();
builder.Services.AddScoped<ISourceBusiness, SourceBusiness>();
builder.Services.AddTransient<IRestProvider, RestProvider>();

// Dependency Injection - SourceItems
builder.Services.AddScoped<ISourceItemRepository, SourceItemRepository>();
builder.Services.AddScoped<ISourceItemBusiness, SourceItemBusiness>();

// Dependency Injection - Settings
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<ISettingBusiness, SettingBusiness>();



var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();