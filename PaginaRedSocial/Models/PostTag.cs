using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaginaRedSocial.Models
{
    public class PostTag
    {
        public int PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public int TagId { get; set; }
        [ForeignKey(nameof(TagId))]

        public Post Post { get; set; }
        public Tag Tag { get; set; }

        public PostTag() { }

        public PostTag(Post post, Tag tag)
        {
            this.Post = post;
            this.Tag = tag;
        }
    }
}
