using Microsoft.EntityFrameworkCore;
using MiniGMAO.Components;
using MiniGMAO.Data;
using MiniGMAO.Services.Implementations;
using MiniGMAO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ── Razor / Blazor ─────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Base de données ────────────────────────────────────────────────────────────
builder.Services.AddDbContext<GmaoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Services métier ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IEquipementService,   EquipementService>();
builder.Services.AddScoped<ITechnicienService,   TechnicienService>();
builder.Services.AddScoped<IInterventionService, InterventionService>();
builder.Services.AddScoped<IDashboardService,    DashboardService>();

// ── Identity ───────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit           = false;
    options.Password.RequiredLength         = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase       = false;
    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<GmaoDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/login";
    options.LogoutPath       = "/api/auth/logout";
    options.AccessDeniedPath = "/access-denied";
    options.ExpireTimeSpan   = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ── Seed : migration + rôles + admin ──────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db      = scope.ServiceProvider.GetRequiredService<GmaoDbContext>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    db.Database.Migrate();

    // Créer les rôles
    foreach (var role in new[] { "Admin", "Technicien" })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    // Compte admin par défaut
    if (await userMgr.FindByEmailAsync("admin@gmao.tn") == null)
    {
        var admin = new IdentityUser
        {
            UserName       = "admin@gmao.tn",
            Email          = "admin@gmao.tn",
            EmailConfirmed = true
        };
        var res = await userMgr.CreateAsync(admin, "Admin123!");
        if (res.Succeeded)
            await userMgr.AddToRoleAsync(admin, "Admin");
    }
}

// ── Pipeline HTTP ──────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   // ← OBLIGATOIRE avant Authorization
app.UseAuthorization();    // ← OBLIGATOIRE
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ── Endpoints auth ─────────────────────────────────────────────────────────────
app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm]     string email,
    [FromForm]     string password) =>
{
    var result = await signInManager.PasswordSignInAsync(
        email, password, isPersistent: false, lockoutOnFailure: true);

    if (result.Succeeded)        return Results.Redirect("/dashboard");
    if (result.IsLockedOut)      return Results.Redirect("/login?error=Compte+verrouillé.+Réessayez+dans+5+minutes");
    return Results.Redirect("/login?error=Email+ou+mot+de+passe+incorrect");

}).DisableAntiforgery();

app.MapPost("/api/auth/logout", async (
    [FromServices] SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.Run();