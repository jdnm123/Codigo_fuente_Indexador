namespace Indexai.Helpers
{
    public enum GridColumn
    {
        Lote,
        Caja,
        NumExpediente,
        Expediente,
        Carpeta,
        Asignado,
        Null
    }

    public static class ColumnHelper
    {
        public static GridColumn GetColumn(string columnName)
        {
            switch (columnName)
            {
                case "t_lote.nom_lote":
                    return GridColumn.Lote;
                case "nro_caja":
                    return GridColumn.Caja;
                case "nro_expediente":
                    return GridColumn.NumExpediente;
                case "nom_expediente":
                    return GridColumn.Expediente;
                case "nro_carpeta":
                    return GridColumn.Carpeta;
                case "Asignado":
                    return GridColumn.Asignado;
            }
            return GridColumn.Null;
        }
    }
}
