using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PLANTINFOWEB.Data;
using Plantpedia.Mapping;
using Plantpedia.Repository;
using Plantpedia.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.PageViewLocationFormats.Insert(0, "/Pages/Partial/{0}.cshtml");
});

var mvcBuilder = builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Home/Home", "");
});

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("ExpertOnly", policy => policy.RequireRole("expert"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAutoMapper(typeof(PlantProfile).Assembly);

// Repository DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();

// Service DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IMesciusReportService, MeciusReportService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "Plantpedia.Auth";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<MyNotificationHub>("/notificationHub");

app.Run();
