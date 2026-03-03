using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using QuestPDF.Infrastructure;

// Configure QuestPDF license
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework
builder.Services.AddDbContext<PSDatos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicializar base de datos con datos de prueba
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<PSDatos>();
        logger.LogInformation("Iniciando seed de base de datos...");
        DbInitializer.Initialize(context);
        logger.LogInformation("Seed completado exitosamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos: {Message}", ex.Message);
        throw; // Re-lanzar para ver el error real en pantalla
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Configurar ruta del logo para reportes PDF
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
PSInventory.Web.Services.PdfReportService.LogoPath =
    Path.Combine(env.WebRootPath, "images", "logo.png");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
