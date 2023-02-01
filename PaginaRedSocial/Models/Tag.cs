using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PaginaRedSocial.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Palabra { get; set; }

        public ICollection<Post> Posts { get; } = new List<Post>();
        public List<PostTag> PostTags { get; set; }

        public Tag() { }

    }
}
