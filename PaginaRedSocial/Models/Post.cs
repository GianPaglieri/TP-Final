using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Metadata;

namespace PaginaRedSocial.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Contenido { get; set; }

        public string Fecha { get; set; }

        //FOREIGN KEY
        public User user { get; set; }
        public int UserId { get; set; }

        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<Reaccion> Reacciones { get; set; } = new List<Reaccion>();

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public List<PostTag> PostTags { get; set; }

        
    }
}
