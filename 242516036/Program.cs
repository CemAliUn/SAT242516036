using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization; // <--- EKLENDÝ (Dil ayarlarý için)
using _242516036.Components;
using _242516036.Components.Account;
using _242516036.Data;
// Bizim Eklediðimiz Namespace'ler
using DbContexts;
using UnitOfWorks;
using Providers;
using QuestPDF.Infrastructure;
using Services;
using _242516036.Services;

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 1. QUESTPDF LÝSANS AYARI (Raporlama için)
// ==============================================================================
QuestPDF.Settings.License = LicenseType.Community;

// ==============================================================================
// 2. DÝL VE SERVÝS AYARLARI (GÜNCELLENDÝ)
// ==============================================================================

// A. Dil Servisi: Kaynak dosyalarýnýn "Resources" klasöründe olduðunu belirtiyoruz
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// B. Controller Desteði: Dil deðiþtirmek (Cookie set etmek) için gerekli
builder.Services.AddControllers();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

// Baðlantý cümlesini alýyoruz
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ==============================================================================
// 3. VERÝTABANI BAÐLANTILARI
// ==============================================================================

// A. Identity (Kullanýcý Giriþ Sistemi)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// B. Bizim Ýþ Mantýðý (Müþteriler, SP'ler)
builder.Services.AddDbContext<MyDbModel_DbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ==============================================================================
// 4. IDENTITY (ÜYELÝK) AYARLARI
// ==============================================================================
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // GÝRÝÞ AYARLARI (RAHATLATILDI)
    options.SignIn.RequireConfirmedAccount = false; // E-posta onayý KAPALI

    // ÞÝFRE AYARLARI
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// ==============================================================================
// 5. BÝZÝM SERVÝSLER (Dependency Injection)
// ==============================================================================
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<MyDbModel_DbContext>>();
builder.Services.AddScoped<IMyDbModel_Provider, MyDbModel_Provider>();

// Loglama Servisi
builder.Services.AddScoped<ISystemLogger, SystemLogger>();

// PDF Raporlama Servisi
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// ==============================================================================
// 6. DÝL AYARLARINI AKTÝF ETME (APP BUILD SONRASI - YENÝ)
// ==============================================================================
var supportedCultures = new[] { "tr", "en" }; // Desteklenen diller
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr") // Varsayýlan dil Türkçe
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// ==============================================================================
// 7. ROUTING (HARÝTALAMA)
// ==============================================================================

// Controller'larý (Dil deðiþimi için) haritala
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// === ROL VE ADMIN OLUÞTURMA (SEED DATA) ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await _242516036.Data.RoleSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // Hata loglama (isteðe baðlý)
        Console.WriteLine(ex.Message);
    }
}

app.Run();