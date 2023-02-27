using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;

namespace PaginaRedSocial.Controllers
{
    public class UsuarioAmigoesController : Controller
    {
        private readonly MyContext _context;
        private SoundPlayer _soundPlayer;

        public UsuarioAmigoesController(MyContext context)
        {
            _context = context;
        }

        // GET: UsuarioAmigoes
        public async Task<IActionResult> Index()
        {
            if (!this._context.Usuarios.Find(int.Parse(User.Identity.Name)).IsAdmin)
                return Redirect("/UsuarioAmigoEs/all?message=No-tenes-permiso-de-administrador");
            var myContext = _context.UsuarioAmigo.Include(u => u.Amigo).Include(u => u.Usuario);
            return View(await myContext.ToListAsync());
        }

        // GET: UsuarioAmigoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioAmigo == null)
            {
                return NotFound();
            }

            var usuarioAmigo = await _context.UsuarioAmigo
                .Include(u => u.Amigo)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarioAmigo == null)
            {
                return NotFound();
            }

            return View(usuarioAmigo);
        }

        // GET: UsuarioAmigoes/Create
        public IActionResult Create()
        {
            ViewData["AmigoId"] = new SelectList(_context.Usuarios, "Id", "Email");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: UsuarioAmigoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,AmigoId")] UsuarioAmigo usuarioAmigo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioAmigo);
                await _context.SaveChangesAsync();
                _soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                _soundPlayer.Play();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AmigoId"] = new SelectList(_context.Usuarios, "Id", "Email", usuarioAmigo.AmigoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", usuarioAmigo.UsuarioId);
            return View(usuarioAmigo);
        }

        // GET: UsuarioAmigoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioAmigo == null)
            {
                return NotFound();
            }

            var usuarioAmigo = await _context.UsuarioAmigo.FindAsync(id);
            if (usuarioAmigo == null)
            {
                return NotFound();
            }
            ViewData["AmigoId"] = new SelectList(_context.Usuarios, "Id", "Email", usuarioAmigo.AmigoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", usuarioAmigo.UsuarioId);
            return View(usuarioAmigo);
        }

        // POST: UsuarioAmigoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,AmigoId")] UsuarioAmigo usuarioAmigo)
        {
            if (id != usuarioAmigo.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioAmigo);
                    _soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                    _soundPlayer.Play();
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioAmigoExists(usuarioAmigo.UsuarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _soundPlayer = new SoundPlayer("Resources/ErrorSound.wav");
                        _soundPlayer.Play();
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AmigoId"] = new SelectList(_context.Usuarios, "Id", "Email", usuarioAmigo.AmigoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", usuarioAmigo.UsuarioId);
            return View(usuarioAmigo);
        }

        // GET: UsuarioAmigoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioAmigo == null)
            {
                return NotFound();
            }

            var usuarioAmigo = await _context.UsuarioAmigo
                .Include(u => u.Amigo)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarioAmigo == null)
            {
                return NotFound();
            }

            return View(usuarioAmigo);
        }

        // POST: UsuarioAmigoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioAmigo == null)
            {
                return Problem("Entity set 'MyContext.UsuarioAmigo'  is null.");
            }
            var usuarioAmigo = await _context.UsuarioAmigo.FindAsync(id);
            if (usuarioAmigo != null)
            {
                _soundPlayer = new SoundPlayer("Resources/DeleteSound.wav");
                _soundPlayer.Play();
                _context.UsuarioAmigo.Remove(usuarioAmigo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioAmigoExists(int id)
        {
            return _context.UsuarioAmigo.Any(e => e.UsuarioId == id);
        }
    }
}
