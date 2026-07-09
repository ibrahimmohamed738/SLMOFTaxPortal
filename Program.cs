using Microsoft.AspNetCore.Authentication.Cookies;
using SLMOFTaxPortal.Security;
using SLMOFTaxPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IMerchantManagementService, MerchantManagementService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

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
    options.AddPolicy(AppPermissions.ViewDashboard, p =>
        p.RequireClaim("Permission", AppPermissions.ViewDashboard));

    options.AddPolicy(AppPermissions.ViewTransactions, p =>
        p.RequireClaim("Permission", AppPermissions.ViewTransactions));

    options.AddPolicy(AppPermissions.ExportTransactions, p =>
        p.RequireClaim("Permission", AppPermissions.ExportTransactions));

    options.AddPolicy(AppPermissions.ViewMerchants, p =>
        p.RequireClaim("Permission", AppPermissions.ViewMerchants));

    options.AddPolicy(AppPermissions.ManageMerchants, p =>
        p.RequireClaim("Permission", AppPermissions.ManageMerchants));

    options.AddPolicy(AppPermissions.ViewUsers, p =>
        p.RequireClaim("Permission", AppPermissions.ViewUsers));

    options.AddPolicy(AppPermissions.ManageUsers, p =>
        p.RequireClaim("Permission", AppPermissions.ManageUsers));

    options.AddPolicy(AppPermissions.ViewRoles, p =>
        p.RequireClaim("Permission", AppPermissions.ViewRoles));

    options.AddPolicy(AppPermissions.ManageRoles, p =>
        p.RequireClaim("Permission", AppPermissions.ManageRoles));

    options.AddPolicy(AppPermissions.ViewPermissions, p =>
        p.RequireClaim("Permission", AppPermissions.ViewPermissions));

    options.AddPolicy(AppPermissions.ManagePermissions, p =>
        p.RequireClaim("Permission", AppPermissions.ManagePermissions));

    options.AddPolicy(AppPermissions.ViewAuditLogs, p =>
        p.RequireClaim("Permission", AppPermissions.ViewAuditLogs));

    options.AddPolicy(AppPermissions.ManageSettings, p =>
        p.RequireClaim("Permission", AppPermissions.ManageSettings));
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
