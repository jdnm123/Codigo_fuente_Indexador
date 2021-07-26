using Gestion.DAL.Models;
using Indexai.Models;
using System;
using System.IO;

namespace Indexai.Helpers
{
    public static class PDFHelper
    {
        public static ArchivoMeta CopiaPdf(CarpetaModel selectedItem, bool mover = false)
        {
            ArchivoMeta archivoMeta = new ArchivoMeta();
            string rutaFuente = GlobalClass.ruta_proyecto + $@"/{selectedItem.t_lote.nom_lote}/{selectedItem.nro_caja}/{selectedItem.nro_expediente}/{selectedItem.nro_expediente}.pdf";
            if (!File.Exists(rutaFuente)) rutaFuente = GlobalClass.ruta_proyecto + $@"/{selectedItem.t_lote.nom_lote}/{selectedItem.nro_caja}/{selectedItem.nro_expediente}.pdf";
            if (!File.Exists(rutaFuente))
            {
                archivoMeta.RutaFuente = rutaFuente;
                archivoMeta.ErrorMsj = "No se encontro el archivo!";
                return archivoMeta;
            }
            string folderCopia = GlobalClass.ruta_proyecto + $@"/Modificados/{selectedItem.t_lote.nom_lote}/{selectedItem.nro_caja}/"; //Carpeta de la copia
            archivoMeta.RutaFuente = rutaFuente;
            archivoMeta.RutaCopia = folderCopia + $@"{selectedItem.nro_expediente}.pdf"; //Archivo última copia
            Directory.CreateDirectory(folderCopia);  //crear Folder si no existe
            if (File.Exists(archivoMeta.RutaCopia)) //si existe al archivo hace copia de la uptima copia
            {
                string oldFileName = archivoMeta.RutaCopia.TrimEnd(".pdf".ToCharArray()) + $"old{Guid.NewGuid()}.pdf";
                File.Move(archivoMeta.RutaCopia, oldFileName);
                File.Delete(archivoMeta.RutaCopia);
            }
            if (mover)
            {
                File.Move(rutaFuente, archivoMeta.RutaCopia); //Mueve el archivo que se está editando
            }
            else
            {
                File.Copy(rutaFuente, archivoMeta.RutaCopia); //Copia el archivo que se está editando
            }

            return archivoMeta;
        }
    }
}
