using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using miPrimerProyecto.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace miPrimerProyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _context.productos
                 .Include(p => p.Categoria)
                 .Include(p => p.Proveedor)
                 .ToListAsync();
            return View(productos);
        }

        public void CargarListasDesplegables()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias?.ToList() ?? new List<Categoria>(), "Id", "Nombre");
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores?.ToList() ?? new List<Proveedor>(), "Id", "Nombre");
        }


        public ActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Nombre, string Precio, int CategoriaId, int ProveedorId)
        {
           
            if (string.IsNullOrEmpty(Precio))
            {
             
                CargarListasDesplegables();
                return View();
            }

            var producto = new Producto
            {
                Nombre = Nombre,
                Precio = Precio, 
                CategoriaId = CategoriaId,
                ProveedorId = ProveedorId
            };

            _context.productos.Add(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.productos.FindAsync(id);
            _context.productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.productos.Any(p => p.Id == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            CargarListasDesplegables();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Nombre, string Precio, int CategoriaId, int ProveedorId)
        {
            var producto = await _context.productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            producto.Nombre = Nombre;
            producto.Precio = Precio;  
            producto.CategoriaId = CategoriaId;
            producto.ProveedorId = ProveedorId;

            try
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Producto actualizado correctamente";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult view()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
