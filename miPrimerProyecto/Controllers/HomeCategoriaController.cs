using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using miPrimerProyecto.Models;
using System.Diagnostics;

namespace miPrimerProyecto.Controllers
{
    public class HomeCategoriaController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeCategoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Productos)
                .ToListAsync();
            return View(categorias);
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

            var Categoria = new Categoria
            {
                Nombre = Nombre
            };

            _context.Categorias.Add(Categoria);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Categoria == null)
            {
                return NotFound();
            }

            return View(Categoria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Categorias.Any(p => p.Id == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            CargarListasDesplegables();
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Nombre)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Nombre = Nombre;

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "categoria actualizado correctamente";
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
