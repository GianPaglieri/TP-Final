using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaginaRedSocial.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public string Contenido { get; set; }
        public DateTime FechaComentario { get; set; }

        // Relaciones
        public User Usuario { get; set; }
        public Post Post { get; set; }

        public Comentario() { }
            public Comentario(string contenido, DateTime fecha)
        {
            this.Usuario = new User();
            this.Post = new Post();
            this.Contenido = contenido;
            this.FechaComentario = fecha;
        }
    }
}
