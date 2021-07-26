using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexai.Models
{
    public class IndiceArchivoDetalle
    {
        public string NombreDocumento { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public System.DateTime fechaIncorporacion { get; set; }
        public int orden { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaInicio { get; set; }
        public int PaginaFin { get; set; }
        public string Formato { get; set; }
        public float tamanio { get; set; }
        public string Origen { get; set; }
        public string Observaciones { get; set; }
    }
}
