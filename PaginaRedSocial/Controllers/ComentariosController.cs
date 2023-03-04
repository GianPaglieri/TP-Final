using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;

namespace PaginaRedSocial.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly MyContext _context;

        public ComentariosController(MyContext context)
        {
            _context = context;
        }

        // GET: Comentarios
       
        public async Task<IActionResult> Index()
        {
            
            if (!this._context.Usuarios.Find(int.Parse(User.Identity.Name)).IsAdmin)
                return Redirect("/Comentarios/all?message=No-tenes-permiso-de-administrador");
            var myContext = _context.comentarios.Include(c => c.Post).Include(c => c.Usuario);
            return View(await myContext.ToListAsync());
        }
        
        // GET: Comentarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.comentarios == null)
            {
                return NotFound();
            }

            var comentario = await _context.comentarios
                .Include(c => c.Post)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }
        public IActionResult CreateComment(Microsoft.AspNetCore.Http.IFormCollection collection)
        {
            int userId = int.Parse(@User.Identity.Name);
            var userActual = this._context.Usuarios
                .Where(user => user.Id == userId)
                .FirstOrDefault();
            Comentario newComment = new Comentario();
            newComment.PostId = int.Parse(collection["PostId"]);       
            newComment.Contenido = collection["ComentarioContent"];
            newComment.Usuario = userActual;
            newComment.FechaComentario = DateTime.Now;
            this._context.comentarios.Add(newComment);
            this._context.SaveChanges();

            return Redirect("/Home");
        }
        // GET: Comentarios/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PostId,UsuarioId,Contenido,FechaComentario")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comentario.PostId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", comentario.UsuarioId);
            return View(comentario);
        }

        // GET: Comentarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.comentarios == null)
            {
                return NotFound();
            }

            var comentario = await _context.comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comentario.PostId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", comentario.UsuarioId);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,UsuarioId,Contenido,FechaComentario")] Comentario comentario)
        {
            if (id != comentario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comentario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(comentario.Id))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comentario.PostId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", comentario.UsuarioId);
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.comentarios == null)
            {
                return NotFound();
            }

            var comentario = await _context.comentarios
                .Include(c => c.Post)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.comentarios == null)
            {
                return Problem("Entity set 'MyContext.comentarios'  is null.");
            }
            var comentario = await _context.comentarios.FindAsync(id);
            if (comentario != null)
            {
                _context.comentarios.Remove(comentario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentarioExists(int id)
        {
            return _context.comentarios.Any(e => e.Id == id);
        }
    }
}
