using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace PaginaRedSocial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(String buscador, DateTime? desdeF, DateTime? hastaF)
        {;
            List<Post> postAmigos = this.getPostsAmigos();
            List<TipoReaccion> tipoReacciones = this._context.tiposReacciones.ToList();

            ViewBag.TipoReacciones = tipoReacciones;

            if (!String.IsNullOrEmpty(buscador))
            {
                int userId = int.Parse(@User.Identity.Name);
                var postsFiltrados = this._context.Posts
                    .Include(u => u.user)
                    .Include(p => p.Comentarios)
                    .Include(p => p.Reacciones)
                    .Where(b => (b.Contenido!.Contains(buscador)) || (b.user.Nombre!.Contains(buscador)));

                foreach (Post post in postsFiltrados)
                {
                    Reaccion existingReaction = post.Reacciones
                                    .Where(reaction => reaction.PostId == post.Id)
                                    .Where(reaction => reaction.UsuarioId == userId)
                                    .FirstOrDefault();
                    if (existingReaction != null)
                    {
                        post.MyReactionId = existingReaction.TipoReaccionId;
                    }
                }

                return View("/Views/Home/Usuarios/Index.cshtml", postsFiltrados);
            }
            else if (desdeF != null || hastaF != null)
            {
                int userId = int.Parse(@User.Identity.Name);
                var postsFiltrados = this._context.Posts
                    .Include(u => u.user)
                    .Include(p => p.Comentarios)
                    .Include(p => p.Reacciones)
                    .Where(b => (b.Fecha > desdeF) || (b.Fecha < hastaF));

                foreach (Post post in postsFiltrados)
                {
                    Reaccion existingReaction = post.Reacciones
                                    .Where(reaction => reaction.PostId == post.Id)
                                    .Where(reaction => reaction.UsuarioId == userId)
                                    .FirstOrDefault();
                    if (existingReaction != null)
                    {
                        post.MyReactionId = existingReaction.TipoReaccionId;
                    }
                }

                return View("/Views/Home/Usuarios/Index.cshtml", postsFiltrados);
            }
            else
            {
                return View("/Views/Home/Usuarios/Index.cshtml", postAmigos);
            }
        }

        private List<Post> getPostsAmigos()
        {
            int userId = int.Parse(@User.Identity.Name);
            var userActual = this._context.Usuarios.Include(u => u.misAmigos)
                .Where(user => user.Id == userId)
                .FirstOrDefault();
            List<Post> postAmigos = new List<Post>();
            List<UsuarioAmigo> amigos = userActual.misAmigos.ToList();
            List<Post> postsFiltrados = this._context.Posts
                                              .Include(p => p.user)
                                              .Include(p => p.Comentarios)
                                              .Include(p => p.Reacciones)
                                              .Where(post => post.UserId != userId)
                                              .OrderByDescending(s => s.Fecha)
                                              .ToList();
            foreach (Post post in postsFiltrados)
            {
                foreach (UsuarioAmigo usuarioAmigo in amigos)
                {
                    if (post.UserId == usuarioAmigo.AmigoId)
                    {
                        Reaccion existingReaction = post.Reacciones
                                        .Where(reaction => reaction.PostId == post.Id)
                                        .Where(reaction => reaction.UsuarioId == userId)
                                        .FirstOrDefault();
                        if (existingReaction != null)
                        {
                            post.MyReactionId = existingReaction.TipoReaccionId;
                        }
                        postAmigos.Add(post);
                    }
                }
            }
            return postAmigos;
        }

        public IActionResult Passnueva(String? message)
        {
            ViewData["message"] = null;
            if (message != null)
                ViewData["message"] = message.Replace("-", " ");
            return View("/Views/Home/MyProfile/Passnueva.cshtml");
        } 


        [Authorize]
        public async Task<IActionResult> Perfil(String? message)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == int.Parse(@User.Identity.Name));

            ViewData["message"] = null;
            if (message != null)
                ViewData["message"] = message.Replace("-", " ");
            return View("/Views/Home/MyProfile/Index.cshtml", user);
        }

        public async Task<IActionResult> MyEdit()
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == int.Parse(@User.Identity.Name));

            return View("/Views/Home/MyProfile/MyEdit.cshtml", user);
        }

        [Authorize]
        public async Task<IActionResult> MisPosts()
        {
            var userActual = this._context.Usuarios.Include(u => u.posts)
                            .Where(user => user.Id == int.Parse(@User.Identity.Name))
                            .FirstOrDefault();
            return View("/Views/Home/MisPosts/Index.cshtml", userActual.posts.OrderByDescending(s => s.Fecha));
        }

        [Authorize]
        public async Task<IActionResult> BuscarAmigos()
        {
            var userActual = this._context.Usuarios.Include(u => u.misAmigos).Where(user => user.Id == int.Parse(@User.Identity.Name)).FirstOrDefault();
            List<User> noAmigos = new List<User>();
            var users = this._context.Usuarios.Where(user => user.IsAdmin != true)
                .Where(user => user.Id != int.Parse(@User.Identity.Name))
                .ToList();
            foreach (User user in users) {
                if (userActual.misAmigos.Count > 0)
                {
                    // busco en mis amigos el usuario filtrado, si el result da 0 significa que
                    // el user no está en mis amigos
                    int result = userActual.misAmigos.Where(ma => ma.AmigoId == user.Id).Count();
                    
                    if (result == 0)
                    {
                        noAmigos.Add(user);
                    }
                }
                else
                {
                    noAmigos.Add(user);
                }
            }
                return View("/Views/Home/BuscarAmigos/Index.cshtml", noAmigos);
        }

        [Authorize]
        public async Task<IActionResult> MisAmigos()
        {
            var userActual = this._context.Usuarios
                            .Include(u => u.misAmigos)
                            .Where(user => user.Id == int.Parse(@User.Identity.Name))
                            .FirstOrDefault();
            List<User> amigos = new List<User>();
            var users = this._context.Usuarios.Where(user => user.IsAdmin != true)
                .Where(user => user.Id != int.Parse(@User.Identity.Name))
                .ToList();
            foreach (User user in users)
            {
                if (userActual.misAmigos.Count > 0)
                {
                    // busco en mis amigos el usuario filtrado, si el result da 0 significa que
                    // el user no está en mis amigos
                    int result = userActual.misAmigos.Where(ma => ma.AmigoId == user.Id).Count();

                    if (result > 0)
                    {
                        amigos.Add(user);
                    }
                }
            }
            return View("/Views/Home/MisAmigos/Index.cshtml", amigos);
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