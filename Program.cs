using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.Mapping;
using Plantpedia.Repository;
using Plantpedia.Service;
using Plantpedia.Settings;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// MVC + API
builder.Services.AddControllers();

var mvcBuilder = builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Home/Home", "");
});
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Views lookup
builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.PageViewLocationFormats.Insert(0, "/Pages/Partial/{0}.cshtml");
});

// AuthZ policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("ExpertOnly", policy => policy.RequireRole("expert"));
});

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Others
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddAutoMapper(typeof(PlantProfile).Assembly);
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// AuthN cookies
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;

        if (builder.Environment.IsDevelopment())
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            options.Cookie.SameSite = SameSiteMode.Lax;
        }
        else
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        }

        options.Cookie.Name = "Plantpedia.Auth";
    });

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IMesciusReportService, MeciusReportService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserFavoriteService, UserFavoriteService>();
builder.Services.AddScoped<IPlantDiagnosisService, PlantDiagnosisService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();

var app = builder.Build();

// Pipeline
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

// Endpoints
app.MapControllers();
app.MapRazorPages();
app.MapHub<MyNotificationHub>("/notificationHub");

app.Run();
