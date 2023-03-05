using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaginaRedSocial.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Contenido { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]

        public DateTime Fecha { get; set; }

        //FOREIGN KEY
        public User user { get; set; }
        public int UserId { get; set; }

        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<Reaccion> Reacciones { get; set; } = new List<Reaccion>();

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public List<PostTag> PostTags { get; set; }

        [NotMapped]
        public int MyReactionId { get; set; }
        public Post() { }
    }
}
