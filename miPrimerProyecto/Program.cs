using Microsoft.EntityFrameworkCore;
using miPrimerProyecto.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configuración de la conexión a la base de datos con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Manejo de errores en producción
    // Habilitar HSTS en producción (protocolo seguro HTTP)
    app.UseHsts();
}

app.UseHttpsRedirection();  // Redirigir tráfico HTTP a HTTPS
app.UseStaticFiles();  // Habilitar archivos estáticos (CSS, JS, imágenes, etc.)
app.UseRouting();  // Activar el enrutamiento para las solicitudes HTTP
app.UseAuthorization();  // Habilitar la autorización para las rutas que lo necesiten

// Configuración de las rutas (URL) de la aplicación
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicación
app.Run();
