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

        
        public async Task<IActionResult> Index()
        {
            var inventario = await _context.Inventarios
                .Include(p => p.Producto)  
                .ToListAsync();
            return View(inventario);
        }

     
        public void CargarListasDesplegables()
        {
            ViewData["ProductoId"] = new SelectList(_context.productos, "Id", "Nombre");
        }


        
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(string cantidad, int ProductoId)
        {
           
            if (string.IsNullOrEmpty(cantidad) || !int.TryParse(cantidad, out int cantidadInt))
            {
                CargarListasDesplegables();
                ModelState.AddModelError("", "La cantidad debe ser un número válido.");
                return View();
            }

         
            var inventario = new Inventario
            {
                Cantidad = cantidadInt,  
                ProductoId = ProductoId,
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(p => p.Producto)  
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
