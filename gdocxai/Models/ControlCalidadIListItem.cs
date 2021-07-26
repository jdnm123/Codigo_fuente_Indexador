using Gestion.DAL;
using System.Collections.Generic;

namespace Indexai.Models
{
    /// <summary>
    /// Modelo creado para facilitar el trabajo del combobox admin.
    /// </summary>
    public class ControlCalidadIListItem
    {
        public bool RequiereSeleccion { get; set; }
        public string Archivador { get; set; }
        public string NoExpediente { get; set; }
        public string NumExpediente { get; set; }
        public string NoCarpeta { get; set; }
        public int Folios { get; set; }
        public string Asignado { get; set; }
        public string Estado { get; set; }
        public p_tipoitem Tipo { get; internal set; }
        public List<t_documento_resp> Respuesta { get; internal set; }
        public List<string> Archivadores { get; internal set; }
        public t_documento Documento { get; internal set; }
        public int Id { get => Documento.id; }
        public string Lote { get; internal set; }
        public string Caja { get; internal set; }
        public int PagIni { get; set; }
        public int PagFin { get; set; }
        public int? SubSerie { get; internal set; }
        public ICollection<t_documento_tercero> Terceros { get; internal set; }
        public int? TotalTerceros { get; internal set; }
        public int CarpetaId { get; internal set; }
        public int? FolioFin { get; internal set; }
        public int? FolioIni { get; internal set; }
        public t_lote LoteModel { get; internal set; }
    }
}
