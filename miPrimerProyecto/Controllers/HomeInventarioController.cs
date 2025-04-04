using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using miPrimerProyecto.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace miPrimerProyecto.Controllers
{
    public class HomeInventarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeInventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar inventario
        public async Task<IActionResult> Index()
        {
            var inventario = await _context.Inventarios
                .Include(p => p.Producto)  // Aseguramos que se incluya la relación con Producto
                .ToListAsync();
            return View(inventario);
        }

        // Cargar lista de productos para el dropdown
        public void CargarListasDesplegables()
        {
            ViewData["ProductoId"] = new SelectList(_context.productos, "Id", "Nombre");
        }


        // Vista para crear nuevo inventario
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        // Acción POST para crear inventario
        [HttpPost]
        public async Task<IActionResult> Create(string cantidad, int ProductoId)
        {
            // Validamos si la cantidad es válida
            if (string.IsNullOrEmpty(cantidad) || !int.TryParse(cantidad, out int cantidadInt))
            {
                CargarListasDesplegables();
                ModelState.AddModelError("", "La cantidad debe ser un número válido.");
                return View();
            }

            // Creamos el nuevo inventario
            var inventario = new Inventario
            {
                Cantidad = cantidadInt,  // Asignamos la cantidad convertida
                ProductoId = ProductoId,
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Eliminar inventario
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(p => p.Producto)  // Incluimos los detalles del Producto
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventarios.Remove(inventario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
            return _context.Inventarios.Any(p => p.Id == id);
        }

        // Editar inventario
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }

            CargarListasDesplegables();
            return View(inventario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Cantidad, int ProductoId)
        {
            var inventario = await _context.Inventarios.FindAsync(id);

            if (inventario == null)
            {
                return NotFound();
            }

            // Actualizamos los valores del inventario
            inventario.Cantidad = Cantidad;
            inventario.ProductoId = ProductoId;

            try
            {
                _context.Update(inventario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Inventario actualizado correctamente";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Vista de privacidad
        public IActionResult Privacy()
        {
            return View();
        }

        // Manejo de errores
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
