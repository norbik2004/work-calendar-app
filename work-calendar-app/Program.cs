using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using work_calendar_app.Services.Entities;
using calendar_app.Common;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Rejestracja serwisów
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

// Rejestracja Identity z UI
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI();

// Konfiguracja DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Konfiguracja middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware dla uwierzytelniania i autoryzacji
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// Mapowanie Razor Pages dla logowania/rejestracji
app.MapRazorPages();

// Domyślny routing MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
