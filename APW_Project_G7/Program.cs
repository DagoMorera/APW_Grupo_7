using APW.Architecture.Providers;
using APW.Mvc.Service;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Servicios base de Mvc
builder.Services.AddControllersWithViews();

// Provider para consumir la Api por HTTP
builder.Services.AddTransient<IRestProvider, RestProvider>();

// Servicios que consumen cada endpoint de la Api
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ISourceService, SourceService>();
builder.Services.AddTransient<ISourceItemService, SourceItemService>();
builder.Services.AddTransient<ISettingService, SettingService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();

// Autenticacion basada en Cookies, sesion del usuario logueado
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();