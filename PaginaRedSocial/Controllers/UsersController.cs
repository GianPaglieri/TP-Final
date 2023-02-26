using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;
using PaginaRedSocial.Helpers;
using System.ComponentModel;

namespace PaginaRedSocial.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyContext _context;

        public UsersController(MyContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (!this._context.Usuarios.Find(int.Parse(User.Identity.Name)).IsAdmin)
                return Redirect("/Users/all?message=No-tenes-permiso-de-administrador");
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dni,Nombre,Email,Password,IsAdmin,Bloqueado,Intentos")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = Utils.Encriptar(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Dni,Nombre,Email,Password,IsAdmin,Bloqueado,Intentos")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        [HttpPost]
        public IActionResult MyEdit(Microsoft.AspNetCore.Http.IFormCollection collection)
        {

            int userId = int.Parse(@User.Identity.Name);
            var userActual = this._context.Usuarios
                .Where(user => user.Id == userId)
                .FirstOrDefault();
            string v = collection["dni"].ToString();
            userActual.Dni = Int32.Parse(v);
            userActual.Nombre = collection["nombre"];
            userActual.Email = collection["email"];
            this._context.Update(userActual);
            this._context.SaveChanges();

            return Redirect("/Home/Perfil");
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'MyContext.Usuarios'  is null.");
            }
            var user = await _context.Usuarios.FindAsync(id);
            if (user != null)
            {
                _context.Usuarios.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AgregarAmigo()
        {
            string page = HttpContext.Request.Query["id"].ToString();
            int id = Int16.Parse(page);            

            var userActual = this._context.Usuarios.Where(user => user.Id == int.Parse(@User.Identity.Name)).FirstOrDefault();
            var nuevoAmigo = this._context.Usuarios.Where(user => user.Id == id).FirstOrDefault();

            UsuarioAmigo userAmigo = new UsuarioAmigo(userActual, nuevoAmigo);
            UsuarioAmigo amigoUser = new UsuarioAmigo(nuevoAmigo, userActual);

            userActual.misAmigos.Add(userAmigo);
            nuevoAmigo.misAmigos.Add(amigoUser);
            this._context.Usuarios.Update(userActual);
            this._context.Usuarios.Update(nuevoAmigo);
            this._context.SaveChanges();

            return Redirect("/Home/BuscarAmigos");
        }

        public async Task<IActionResult> EliminarAmigo()
        {
            string queryId = HttpContext.Request.Query["id"].ToString();
            int exAmigoId = Int16.Parse(queryId);
            int userId = int.Parse(@User.Identity.Name);

            User exAmigo = this._context.Usuarios.Include(u => u.misAmigos).Where(u => u.Id == exAmigoId).FirstOrDefault();
            var userActual = this._context.Usuarios.Include(u => u.misAmigos).Where(user => user.Id == userId).FirstOrDefault();

            UsuarioAmigo exAmigoUser = exAmigo.misAmigos
                                    .Where(ua => ua.UsuarioId == exAmigoId && ua.AmigoId == userActual.Id)
                                    .FirstOrDefault();
            UsuarioAmigo userExAmigo = userActual.misAmigos
                                .Where(ua => ua.UsuarioId == userActual.Id && ua.AmigoId == exAmigoId)
                                .FirstOrDefault();

            userActual.misAmigos.Remove(userExAmigo);
            exAmigo.misAmigos.Remove(exAmigoUser);

            this._context.Usuarios.Update(userActual);
            this._context.Usuarios.Update(exAmigo);
            this._context.SaveChanges();

            return Redirect("/Home/MisAmigos");
        }

    }
}
