using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PaginaRedSocial.Models
{
    public class UsuarioAmigo
    {
            public int UsuarioId { get; set; }
            [ForeignKey(nameof(UsuarioId))]
            public User Usuario { get; set; }

            public int AmigoId { get; set; }
            [ForeignKey(nameof(AmigoId))]
            public User Amigo { get; set; }

            public UsuarioAmigo() { }

            public UsuarioAmigo(User principal, User segundo)
            {
                this.Usuario = principal;
                this.Amigo = segundo;
            }
         
    }
}
