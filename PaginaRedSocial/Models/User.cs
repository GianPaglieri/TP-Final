
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PaginaRedSocial.Models
{
    public class User
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [Range(100000, 99999999, ErrorMessage = "El DNI debe contener entre 6 y 8 caracteres")]
        public int Dni { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(2, ErrorMessage = "El nombre debe contener mas de 3 caracteres")]
        public String Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Escriba un email valido")]
        public String Email { get; set; }

        [Required(ErrorMessage = "La password es obligatoria")]
        public String Password { get; set; }

        public bool IsAdmin { get; set; }
        public bool Bloqueado { get; set; }
        public int Intentos { get; set; }

        public List<Post> posts { get; } = new List<Post>();
        public List<Comentario> MisComentarios { get; set; } = new List<Comentario>();

        public List<Reaccion> MisReacciones { get; set; } = new List<Reaccion>();

        public virtual ICollection<UsuarioAmigo> misAmigos { get; set; } = new List<UsuarioAmigo>();
        public virtual ICollection<UsuarioAmigo> amigosMios { get; set; } = new List<UsuarioAmigo>();

        public User() { }
    }
}

