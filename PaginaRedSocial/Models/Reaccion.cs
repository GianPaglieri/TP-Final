using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PaginaRedSocial.Models;

namespace PaginaRedSocial
{
    public class Reaccion
    {
        public int Id { get; set; }
        public int TipoReaccionId { get; set; }
        [ForeignKey(nameof(TipoReaccionId))]
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public int PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public TipoReaccion TipoReaccion { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public Reaccion() { }

        public enum Tipo
        {
            Me_Gusta = 1,
            Me_Encanta = 2,
            Me_Importa = 3,
            Me_Divierte = 4,
            Me_Asombra = 5,
            Me_Entristece = 6,
            Me_Enoja = 7,
        }

    }
}
