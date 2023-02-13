using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PaginaRedSocial.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Media;
using PaginaRedSocial.Data;
using PaginaRedSocial.Helpers;

namespace PaginaRedSocial.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyContext _context;
        

        public LoginController(MyContext context)
        {
            this._context = context;
        }

        [HttpGet("Login")]
        public IActionResult Index(String? message)
        {
            ViewData["message"] = null;
            if (message != null)
                ViewData["message"] = message.Replace("-", " ");
            return View();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> LoginAsync(int dni, String password)
        {
            // Validar el DNI
            var usuario = this._context.Usuarios.Where(user => user.Dni == dni).FirstOrDefault();
            var passwordEncriptada = Utils.Encriptar(password);

            // Valido el DNI
            if (usuario == null)
            {
                
                return Redirect("/Login?message=El-DNI-ingresado-es-incorrecto");
            }

            // Verifico intentos
            if (usuario.Bloqueado)
            {
                
                return Redirect("/Login?message=Su-usuario-esta-bloqueado-por-superar-el-limite-de-intentos-permitidos");
            }

            // Validar la contraseña e Incrementar intentos.
            if (usuario.Password != passwordEncriptada)
            {
                usuario.Intentos++;

                // Bloquear usuario por intentos
                if (usuario.Intentos >= 3)
                    usuario.Bloqueado = true;

                //usuario.Intentos = 0;
                this._context.Update(usuario);
                this._context.SaveChanges();

               
                return Redirect("/Login?message=La-clave-ingresada-es-incorrecta");
            }

            // LOGEO AL USUARIO
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.IsAdmin ? "Admin" : "User"),
                new Claim("Usuario", usuario.Nombre),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Login");

            // Propiedades de la Autenticacion
            var authProperties = new AuthenticationProperties { ExpiresUtc = DateTimeOffset.Now.AddMinutes(60) };

            // Autenticar usuario
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            if (usuario.IsAdmin)
            {
                // ADMIN
              /*  _soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                _soundPlayer.Play();
                usuario.Intentos = 0;*/
                this._context.Update(usuario);
                this._context.SaveChanges();
                return Redirect("/Users");
            }
            else
            {
                // USUARIO CLIENTE
                /*_soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                _soundPlayer.Play();
                usuario.Intentos = 0;*/
                this._context.Update(usuario);
                this._context.SaveChanges();
                return Redirect("/Home");
            }
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            /*_soundPlayer = new SoundPlayer("Resources/DeleteSound.wav");
            _soundPlayer.Play();*/
            return RedirectToAction("Login");
        }
    }
}

