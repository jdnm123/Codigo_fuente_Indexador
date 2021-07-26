using System;

namespace Indexai.Models
{
    public class ControlCalidadAsignadoModel
    {
        internal int id { get; set; }
        public string Lote { get; set; }
        public string NroExpediente { get; set; }
        public string NomExpediente { get; set; }
        public string Caja { get; set; }
        public int NroCarpeta { get; set; }
        public string hc_ini { get; set; }
        public string hc_fin { get; set; }
        public int TotalFolios { get; set; }
        public bool AsignadoControlCalidad { get; internal set; }
        public string Realizo { get; internal set; }
        public DateTime? FechaIndexado { get; set; }

        public string FechaIndexadoFormat { get => FechaIndexado?.ToString("dd-MM-yyyy"); }
    }
}
