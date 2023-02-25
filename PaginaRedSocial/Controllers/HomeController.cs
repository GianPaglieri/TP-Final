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
        public IActionResult Index()
        {
            List<Post> postAmigos = this.getPostsAmigos();

            return View("/Views/Home/Usuarios/Index.cshtml", postAmigos);
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
                                              .Where(post => post.UserId != userId)
                                              .ToList();
            foreach (Post post in postsFiltrados)
            {
                foreach (UsuarioAmigo usuarioAmigo in amigos)
                {
                    if (post.UserId == usuarioAmigo.AmigoId)
                    {
                        postAmigos.Add(post);
                    }
                }
            }
            return postAmigos;
        }

        public IActionResult Perfil()
        {
            return View("/Views/Home/MyProfile/Index.cshtml");
        }

        public IActionResult Passnueva()
        {
            return View("/Views/Home/MyProfile/Passnueva.cshtml");
        } 


        [Authorize]
        public async Task<IActionResult> MisPosts()
        {
            var userActual = this._context.Usuarios.Include(u => u.posts)
                            .Where(user => user.Id == int.Parse(@User.Identity.Name))
                            .FirstOrDefault();

            return View("/Views/Home/MisPosts/Index.cshtml", userActual.posts);
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