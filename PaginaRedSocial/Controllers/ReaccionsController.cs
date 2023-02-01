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
    public class ReaccionsController : Controller
    {
        private readonly MyContext _context;

        public ReaccionsController(MyContext context)
        {
            _context = context;
        }

        // GET: Reaccions
        public async Task<IActionResult> Index()
        {
            var myContext = _context.reacciones.Include(r => r.Post).Include(r => r.TipoReaccion).Include(r => r.User);
            return View(await myContext.ToListAsync());
        }

        // GET: Reaccions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reacciones == null)
            {
                return NotFound();
            }

            var reaccion = await _context.reacciones
                .Include(r => r.Post)
                .Include(r => r.TipoReaccion)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reaccion == null)
            {
                return NotFound();
            }

            return View(reaccion);
        }

        // GET: Reaccions/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            ViewData["TipoReaccionId"] = new SelectList(_context.tiposReacciones, "Id", "Id");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Reaccions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TipoReaccionId,UsuarioId,PostId")] Reaccion reaccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reaccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", reaccion.PostId);
            ViewData["TipoReaccionId"] = new SelectList(_context.tiposReacciones, "Id", "Id", reaccion.TipoReaccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reaccion.UsuarioId);
            return View(reaccion);
        }

        // GET: Reaccions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reacciones == null)
            {
                return NotFound();
            }

            var reaccion = await _context.reacciones.FindAsync(id);
            if (reaccion == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", reaccion.PostId);
            ViewData["TipoReaccionId"] = new SelectList(_context.tiposReacciones, "Id", "Id", reaccion.TipoReaccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reaccion.UsuarioId);
            return View(reaccion);
        }

        // POST: Reaccions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipoReaccionId,UsuarioId,PostId")] Reaccion reaccion)
        {
            if (id != reaccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reaccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaccionExists(reaccion.Id))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", reaccion.PostId);
            ViewData["TipoReaccionId"] = new SelectList(_context.tiposReacciones, "Id", "Id", reaccion.TipoReaccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reaccion.UsuarioId);
            return View(reaccion);
        }

        // GET: Reaccions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reacciones == null)
            {
                return NotFound();
            }

            var reaccion = await _context.reacciones
                .Include(r => r.Post)
                .Include(r => r.TipoReaccion)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reaccion == null)
            {
                return NotFound();
            }

            return View(reaccion);
        }

        // POST: Reaccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reacciones == null)
            {
                return Problem("Entity set 'MyContext.reacciones'  is null.");
            }
            var reaccion = await _context.reacciones.FindAsync(id);
            if (reaccion != null)
            {
                _context.reacciones.Remove(reaccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaccionExists(int id)
        {
            return _context.reacciones.Any(e => e.Id == id);
        }
    }
}
