using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;
using System.Diagnostics;

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
            return View("/Views/Home/Usuarios/Index.cshtml");
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
                System.Console.WriteLine("count amigos: " + userActual.misAmigos.Count);
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
                    System.Console.WriteLine("Entró al else");
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