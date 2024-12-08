using librex3.Data;
using librex3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Konfiguracja ASP.NET Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Dodanie wsparcia dla r�l
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Dodanie MVC z widokami Razor
builder.Services.AddControllersWithViews();

var app = builder.Build();

// �rodowisko middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Middleware do obs�ugi uwierzytelniania
app.UseAuthorization();  // Middleware do obs�ugi autoryzacji

// Definiowanie domy�lnego routingu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Obs�uga obszar�w Razor Pages (np. logowanie/rejestrowanie)


// Tworzenie r�l i domy�lnego administratora
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Tworzenie roli Admin
    var adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Opcjonalne: Tworzenie domy�lnego konta administratora
    var adminEmail = "admin@librex.com";
    var adminPassword = "Admin123!";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null) // Sprawdzenie, czy administrator istnieje
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Administrator"
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }

}

app.Run();
