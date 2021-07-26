using System;

namespace Indexai.Models
{
    class DocumentoGeneral
    {
        public Nullable<int> item { get; set; }
        public Nullable<int> folio_ini { get; set; }
        public Nullable<int> folio_fin { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public string nro_doc { get; set; }
        public string nom_doc { get; set; }
        public string observacion { get; set; }
        public System.DateTime fecha_regdoc { get; set; }
    }
}
