using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PaginaRedSocial
{
    public class TipoReaccion
    {
        public int Id { get; set; }
        public string Palabra { get; set; }

        public List<Reaccion> Reacciones { get; set; }

        public TipoReaccion() { }
    }
}
