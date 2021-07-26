using Gestion.DAL;

namespace Indexai.Models
{
    public class Beneficiarios
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public bool sol_principal { get; set; }
        public t_tercero GeneratedEntity { get; internal set; }
        public t_documento_tercero DocumentoTercero { get; internal set; }
    }
}
