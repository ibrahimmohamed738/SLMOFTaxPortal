using Microsoft.AspNetCore.Authentication.Cookies;
using SLMOFTaxPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IMerchantManagementService, MerchantManagementService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewDashboard", policy =>
        policy.RequireClaim("Permission", "ViewDashboard"));

    options.AddPolicy("ViewTransactions", policy =>
        policy.RequireClaim("Permission", "ViewTransactions"));

    options.AddPolicy("ExportTransactions", policy =>
        policy.RequireClaim("Permission", "ExportTransactions"));

    options.AddPolicy("ViewMerchants", policy =>
        policy.RequireClaim("Permission", "ViewMerchants"));

    options.AddPolicy("ManageMerchants", policy =>
        policy.RequireClaim("Permission", "ManageMerchants"));

    options.AddPolicy("ViewUsers", policy =>
        policy.RequireClaim("Permission", "ViewUsers"));

    options.AddPolicy("ManageUsers", policy =>
        policy.RequireClaim("Permission", "ManageUsers"));

    options.AddPolicy("ManageSettings", policy =>
        policy.RequireClaim("Permission", "ManageSettings"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
