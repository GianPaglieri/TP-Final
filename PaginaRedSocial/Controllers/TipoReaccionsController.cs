using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;

namespace PaginaRedSocial.Controllers
{
    public class TipoReaccionsController : Controller
    {
        private readonly MyContext _context;

        public TipoReaccionsController(MyContext context)
        {
            _context = context;
        }

        // GET: TipoReaccions
        public async Task<IActionResult> Index()
        {
            if (!this._context.Usuarios.Find(int.Parse(User.Identity.Name)).IsAdmin)
                return Redirect("/TipoReacciones/all?message=No-tenes-permiso-de-administrador");
            return View(await _context.tiposReacciones.ToListAsync());
        }

        // GET: TipoReaccions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tiposReacciones == null)
            {
                return NotFound();
            }

            var tipoReaccion = await _context.tiposReacciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoReaccion == null)
            {
                return NotFound();
            }

            return View(tipoReaccion);
        }

        // GET: TipoReaccions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoReaccions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Palabra")] TipoReaccion tipoReaccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoReaccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoReaccion);
        }

        // GET: TipoReaccions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tiposReacciones == null)
            {
                return NotFound();
            }

            var tipoReaccion = await _context.tiposReacciones.FindAsync(id);
            if (tipoReaccion == null)
            {
                return NotFound();
            }
            return View(tipoReaccion);
        }

        // POST: TipoReaccions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Palabra")] TipoReaccion tipoReaccion)
        {
            if (id != tipoReaccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoReaccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoReaccionExists(tipoReaccion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoReaccion);
        }

        // GET: TipoReaccions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tiposReacciones == null)
            {
                return NotFound();
            }

            var tipoReaccion = await _context.tiposReacciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoReaccion == null)
            {
                return NotFound();
            }

            return View(tipoReaccion);
        }

        // POST: TipoReaccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tiposReacciones == null)
            {
                return Problem("Entity set 'MyContext.tiposReacciones'  is null.");
            }
            var tipoReaccion = await _context.tiposReacciones.FindAsync(id);
            if (tipoReaccion != null)
            {
                _context.tiposReacciones.Remove(tipoReaccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoReaccionExists(int id)
        {
            return _context.tiposReacciones.Any(e => e.Id == id);
        }
    }
}
