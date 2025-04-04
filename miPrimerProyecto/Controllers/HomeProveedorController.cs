using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using miPrimerProyecto.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace miPrimerProyecto.Controllers
{
    public class HomeProveedorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeProveedorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var proveedores = await _context.Proveedores
                .Include(c => c.Productos)
                .ToListAsync();
            return View(proveedores);
        }

        
          public void CargarListasDesplegables()
        {
            ViewData["ProductoId"] = new SelectList(_context.productos?.ToList() ?? new List<Producto>(), "Id", "Nombre");
        }


      
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                CargarListasDesplegables();
                return View();
            }

            var proveedor = new Proveedor
            {
                Nombre = Nombre
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.Id == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(p => p.Id == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            CargarListasDesplegables();
            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Nombre)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            proveedor.Nombre = Nombre;

            try
            {
                _context.Update(proveedor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Proveedor actualizado correctamente";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
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

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}