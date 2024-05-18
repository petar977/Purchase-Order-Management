using BussinesLayer.Managers;
using BussinesLayer.MapperProfiles;
using DataLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Po.Common.Interfaces;
using Po.Common.Utilities;
using Serilog;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()  
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(PurchaseOrderProfile)));
builder.Services.AddDbContext<PoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<PoDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    //options.LoginPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.Events.OnSigningIn = (signinContext) => {
        // you can use the pricipal to query claims 
        var email = signinContext.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
        if ("xxxx@hotmail.com".Equals(email))
        {
            // set the expiration time according to claims dynamically 
            signinContext.Properties.ExpiresUtc = DateTimeOffset.Now.AddSeconds(100);
            signinContext.CookieOptions.Expires = signinContext.Properties.ExpiresUtc?.ToUniversalTime();
        }
        else
        {
            signinContext.Properties.ExpiresUtc = DateTimeOffset.Now.AddMinutes(60);
            signinContext.CookieOptions.Expires = signinContext.Properties.ExpiresUtc?.ToUniversalTime();
        }

        return Task.CompletedTask;
    };
});

var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IPurchaseOrderManager, PurchaseOrderManager>();
builder.Services.AddScoped<IPurchaseOrderItemsManager,PurchaseOrderItemsManager > ();
builder.Services.AddScoped<IItemsManager, ItemsManager>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<ISenderEmail, SenderEmail>();
builder.Services.AddScoped<ICompanyManager, CompanyManager>();
builder.Services.AddScoped<IUsersManagementManager, UsersManagementManager>();
builder.Services.AddScoped<IOrderCountManager, OrderCountManager>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PurchaseOrder}/{action=Index}/{id?}");

app.Run();
