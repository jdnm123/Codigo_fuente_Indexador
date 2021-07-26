using Gestion.DAL;
using System.Collections.Generic;

namespace Indexai.Models
{
    /// <summary>
    /// Clase modelo para el grid de búsqueda.
    /// </summary>
    public class BusquedaModel
    {
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string MarcoLegal { get; set; }
        public string TipoDocumental { get; set; }
        public int FolioInicial { get; set; }
        public string FUD { get; set; }
        public string Caja { get; set; }
        public string Expediente { get; set; }
        public t_lote Lote { get; internal set; }
        public string NumExpediente { get; internal set; }
        public int? PagIni { get; internal set; }
        public int? PagFin { get; internal set; }
    }
}
