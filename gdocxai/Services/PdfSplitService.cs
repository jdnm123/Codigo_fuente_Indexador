using Gestion.DAL;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Alphaleonis.Win32.Filesystem;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Indexai.Models;
using System.IO.Compression;
using Syncfusion.OCRProcessor;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Directory = Alphaleonis.Win32.Filesystem.Directory;

namespace Indexai.Services
{
    class PdfSplitService
    {
        private PdfExportService _exportService;

        public async         Task
pdf2docs(int idCarpeta,bool hc,int ocr, bool sobreescribir)
        {   //   \s+([^\s]+)|([^-]*)$|(\-)
            _exportService = new PdfExportService();

            string nomLote = "", nroCaja = "", nroExpediente = "", nomExpediente = string.Empty, codTipoDoc = "", nomTipoDoc = "", nomSubDependencia = "", codSubDependencia = "", codSubSerie = "", nomSubSerie = "", codSerie = "", nomSerie = "", nro_doc = string.Empty, nombres = "", apellidos = "", identificacion = "", nomTipoDocExport= string.Empty;
            string rutaExportar = GlobalClass.ruta_salida?.Trim() + @"/" + GlobalClass.estructura_export + "/";
            string resultString = string.Empty;
            string pattern = "";
            int item = 0;
            //Rama Judicial
            IndiceArchivoMaestro iaMaestro = new IndiceArchivoMaestro();
            List<IndiceArchivoDetalle> ListDetalle = new List<IndiceArchivoDetalle>();

            string nombreExportar = GlobalClass.nombre_export?.Replace(" ", String.Empty);
            if (string.IsNullOrEmpty(GlobalClass.estructura_export)) rutaExportar = GlobalClass.ruta_salida?.Trim() + "/Exportados/{nomLote}/{nroCaja}/{nroExpediente}/";
            if (nombreExportar == null) nombreExportar = "{serial}";
            var res = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").AsNoTracking().Where(r => r.id == idCarpeta).FirstOrDefault();
            if (res != null)
            {
                nomExpediente = res.nom_expediente;
                /*if (!string.IsNullOrEmpty(res.nro_expediente))
                {
                    EntitiesRepository.Context.Database.ExecuteSqlCommand("exec [dbo].[asignaItemExpediente] @nro_expediente", new SqlParameter("@nro_expediente", res.nro_expediente));
                }*/
                string[] result = rutaExportar.Split('{', '}');
                //Nombre del Lote
                if (!string.IsNullOrEmpty(res.t_lote.nom_lote))
                {
                    nomLote = res.t_lote.nom_lote.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{nomLote}", nomLote);
                    nombreExportar = Regex.Replace(nombreExportar, "{nomLote}", nomLote);
                }
                //Número de Caja
                if (!string.IsNullOrEmpty(res.nro_caja))
                {
                    nroCaja = res.nro_caja.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{nroCaja}", nroCaja);
                    nombreExportar = Regex.Replace(nombreExportar, "{nroCaja}", nroCaja);
                }
                //Número de Expediente
                if (!string.IsNullOrEmpty(res.nro_expediente))
                {
                    var txtOrigen = "{nroExpediente}";
                    foreach (var s in result)
                    {
                        if (s.Contains("nroExpedienteREGEX"))
                        {
                            txtOrigen = "{" + s+ "}";
                            var dividido = s.Split(new string[] { "REGEX" }, StringSplitOptions.None);
                            if (dividido.Length > 1) pattern = dividido[1];
                        }

                    }
                    nroExpediente = Regex.Replace(res.nro_expediente, pattern, "");
                    rutaExportar = rutaExportar.Replace($@"{txtOrigen}", nroExpediente);
                    nombreExportar = nombreExportar.Replace($@"{txtOrigen}", nroExpediente);
                }
                //Código de Subdependencia
                if (!string.IsNullOrEmpty(res.t_lote.p_subdependencia.cod))
                {
                    codSubDependencia = res.t_lote.p_subdependencia.cod.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{codSubDependencia}", codSubDependencia);
                    nombreExportar = Regex.Replace(nombreExportar, "{codSubDependencia}", codSubDependencia);
                }
                //Nombre Subdependencia
                if (!string.IsNullOrEmpty(res.t_lote.p_subdependencia.nombre))
                {
                    nomSubDependencia = res.t_lote.p_subdependencia.nombre.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{nomSubDependencia}", nomSubDependencia);
                    nombreExportar = Regex.Replace(nombreExportar, "{nomSubDependencia}", nomSubDependencia);
                }
                //Código de Serie
                if (!string.IsNullOrEmpty(res.t_lote.p_subserie.p_serie.codigo))
                {
                    codSerie = res.t_lote.p_subserie.p_serie.codigo.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{codSerie}", codSerie);
                    nombreExportar = Regex.Replace(nombreExportar, "{codSerie}", codSerie);
                }
                //Nombre de Serie
                if (!string.IsNullOrEmpty(res.t_lote.p_subserie.p_serie.nombre))
                {
                    nomSerie = res.t_lote.p_subserie.p_serie.nombre.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{nomSerie}", nomSerie);
                    nombreExportar = Regex.Replace(nombreExportar, "{nomSerie}", nomSerie);
                }
                //Código Subserie
                if (!string.IsNullOrEmpty(res.t_lote.p_subserie.codigo))
                {
                    codSubSerie = res.t_lote.p_subserie.codigo.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{codSubSerie}", codSubSerie);
                    nombreExportar = Regex.Replace(nombreExportar, "{codSubSerie}", codSubSerie);
                }
                //Nombre Subserie
                if (!string.IsNullOrEmpty(res.t_lote.p_subserie.nombre))
                {
                    nomSubSerie = res.t_lote.p_subserie.nombre.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{nomSubSerie}", nomSubSerie);
                    nombreExportar = Regex.Replace(nombreExportar, "{nomSubSerie}", nomSubSerie);
                }
                //Nombres de tercero
                if (!string.IsNullOrEmpty(res.t_tercero?.nombres))
                {
                    nombres = res.t_tercero.nombres.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{nombres}", nombres);
                    nombreExportar = Regex.Replace(nombreExportar, "{nombres}", nombres);
                }
                //Apellidos de tercero
                if (!string.IsNullOrEmpty(res.t_tercero?.apellidos))
                {
                    apellidos = res.t_tercero.apellidos.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{apellidos}", apellidos);
                    nombreExportar = Regex.Replace(nombreExportar, "{apellidos}", apellidos);
                }
                //Identificación de tercero
                if (!string.IsNullOrEmpty(res.t_tercero?.identificacion))
                {
                    identificacion = res.t_tercero.identificacion.Trim();
                    rutaExportar = Regex.Replace(rutaExportar, "{identificacion}", identificacion);
                    nombreExportar = Regex.Replace(nombreExportar, "{identificacion}", identificacion);
                }

                //RUTA ORIGEN
                string folderPDF = GlobalClass.ruta_proyecto + $@"/{nomLote}/{nroCaja}/{res.nro_expediente}/";
                string ruta = folderPDF + $@"{res.nro_expediente}.pdf";
                if (!System.IO.File.Exists(ruta)) folderPDF = GlobalClass.ruta_proyecto + $@"/{nomLote}/{nroCaja}/";
                ruta = folderPDF + $@"{res.nro_expediente}.pdf";
                string rutaFinal = string.Empty;
                if (System.IO.File.Exists(ruta)) //Si existe el archivo
                {
                    //SI existe la ruta Elimina los archivos anteriores
                    if (System.IO.Directory.Exists(rutaExportar))
                    {
                        /*System.IO.DirectoryInfo di = new DirectoryInfo(rutaExportar);
                        foreach (FileInfo file in di.GetFiles("*.pdf", SearchOption.AllDirectories))
                        {
                            file.Delete();
                        }*/
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(rutaExportar);
                    }
                    PdfLoadedDocument oldPdfDoc = new PdfLoadedDocument(ruta); //Abre PDF Original
                    int totalPags = oldPdfDoc.Pages.Count;
                    int serial = -1, principal = -1, paginas = 0;
                    bool excluir = false;
                    var preDoc = EntitiesRepository.Entities.t_documento.Include("p_tipodoc").AsNoTracking().Where(r => r.id_carpeta == idCarpeta);

                    System.Collections.Generic.List<Gestion.DAL.t_documento> resDoc = new System.Collections.Generic.List<Gestion.DAL.t_documento>();
                    if (GlobalClass.nom_proyecto.Trim().ToUpper().Contains("RAMA"))
                    {   //Si es Rama Judicial
                        iaMaestro.ciudad = nomLote;
                        iaMaestro.despacho = nomSubDependencia;
                        iaMaestro.SerieSubserie = nomSubSerie;
                        iaMaestro.NroRadicacion = nroExpediente;
                        string[] personas = getTerceros(idCarpeta).Split('|');
                        iaMaestro.ParteA = "NO ENCONTRADO";
                        iaMaestro.ParteB = "NO ENCONTRADO";
                        if(personas.Length > 0) iaMaestro.ParteA = personas[0];
                        if (personas.Length > 1) iaMaestro.ParteB = personas[1];
                        iaMaestro.Cuaderno = "1";
                        iaMaestro.NroCarpeta = "1";
                        resDoc = preDoc.OrderBy(x => x.item).ThenBy(x => x.id).ToList();
                    }
                    else
                    {
                        resDoc = preDoc.OrderBy(x => x.pag_ini).ToList();
                    }
                    string txtOriginalSerial = "{serial}";
                    rutaFinal = rutaExportar;
                    //SI TIENE LA OPCIÓN DE SOBREESCRIBIR ELIMINA LOS PDF ANTERIORES
                    if (sobreescribir && GlobalClass.nom_proyecto.Trim().ToUpper().Contains("RAMA"))
                    {
                        var _files = Directory.EnumerateFiles(rutaFinal, "*.pdf", SearchOption.AllDirectories).Reverse().ToList();

                        foreach (string archivoCache in _files)
                        {
                            FileInfo fi = new FileInfo(archivoCache);
                            fi.Delete();
                        }
                    }
                    //BUCLE POR CADA TIPOLOGÍA DOCUMENTAL
                    foreach (var r in resDoc)
                    {
                        string nomArchivoTipo = nombreExportar;
                        string[] resultArchivo = nomArchivoTipo.Split('{', '}');

                        rutaFinal = rutaExportar;
                        codTipoDoc = string.Empty;
                        nomTipoDoc = string.Empty;
                        nroExpediente = string.Empty;
                        item = 0;
                        if(r.p_tipodoc == null) continue;
                        int idDoc = r.id;
                        principal = r.p_tipodoc.principal;
                        excluir = r.p_tipodoc.excluir;
                        //IDENTIFICA SERIAL
                        if (serial == -1)
                        {
                            int tmpSerial = 0;
                            foreach (var s in resultArchivo)
                            {
                                if (s.Contains("serialN"))
                                {
                                    txtOriginalSerial = "{" + s + "}";
                                    var dividido = s.Split(new string[] { "N" }, StringSplitOptions.None);
                                    if (dividido.Length > 1) tmpSerial = GlobalClass.GetNumber(dividido[1]);
                                }
                            }
                            if (!excluir) serial = tmpSerial + 1;
                            else serial = tmpSerial;
                        }
                        //SERIAL
                        nomArchivoTipo = Regex.Replace(nomArchivoTipo, txtOriginalSerial, serial.ToString().PadLeft(2, '0'));

                        if (!string.IsNullOrEmpty(r.p_tipodoc.cod))
                        {
                            codTipoDoc = r.p_tipodoc.cod;
                            nomArchivoTipo = Regex.Replace(nomArchivoTipo, "{codTipoDoc}", codTipoDoc);
                        }
                        if (!string.IsNullOrEmpty(r.p_tipodoc.nombre))
                        {
                            nomTipoDoc = r.p_tipodoc.nombre.Replace(@"/", " ");
                            nomArchivoTipo = Regex.Replace(nomArchivoTipo, "{nomTipoDoc}", nomTipoDoc);
                        }

                        if (!string.IsNullOrEmpty(r.p_tipodoc.nom_doc_alias))
                        {
                            nomTipoDocExport = r.p_tipodoc.nom_doc_alias.Replace(@"/", " ");
                            nomArchivoTipo = Regex.Replace(nomArchivoTipo, "{nom_doc_alias}", nomTipoDocExport);
                        }
                        //DOCUMENTO - NÚMERO DE DOCUMENTO
                        if (!string.IsNullOrEmpty(r.nro_doc) && principal == 1)
                        {
                            nro_doc = r.nro_doc.Trim();
                            //serial = 1;
                        }
                        if (nro_doc != r.nro_doc) r.nro_doc = nro_doc;
                        if (!string.IsNullOrEmpty(r.nro_doc)){
                            nomArchivoTipo = Regex.Replace(nomArchivoTipo, "{nro_doc}", nro_doc);
                            rutaFinal = Regex.Replace(rutaFinal, "{nro_doc}", nro_doc);
                        }

                        if (r.item != null && r.item>0)
                        {
                            item = GlobalClass.GetNumber(r.item?.ToString(), 1);
                            nomArchivoTipo = Regex.Replace(nomArchivoTipo, "{item}", item.ToString().PadLeft(3, '0'));
                        }

                        nomArchivoTipo += ".pdf";

                        string rutaCompleta = rutaFinal + nomArchivoTipo;

                        int pagIni = GlobalClass.GetNumber(r.pag_ini.ToString(), 1) - 1;
                        int pagFin = GlobalClass.GetNumber(r.pag_fin.ToString(), totalPags) - 1;
                        if (pagIni > (totalPags - 1)) pagIni = totalPags - 1;   //Si el MAX es mayor el numero de páginas, hace el ajuste
                        if (pagFin > (totalPags - 1)) pagFin = totalPags - 1;   //Si el MAX es mayor el numero de páginas, hace el ajuste
                        if (pagIni < 0) pagIni = 0;   //Si es menor a cero hacer el ajuste, hace el ajuste
                        if (pagFin < 0) pagFin = 0;   //Si es menor a cero hacer el ajuste, hace el ajuste
                        if (pagIni > pagFin)
                        {
                            int enTmp = pagFin;
                            pagFin = pagIni;
                            pagIni = enTmp;
                        }

                        //REALIZA RESUMEN DE LA EXPORTACIÓN
                        paginas++;
                        IndiceArchivoDetalle iaDetalle = new IndiceArchivoDetalle();
                        iaDetalle.NombreDocumento = nomTipoDoc;
                        iaDetalle.fechaCreacion = r.fecha_regdoc;
                        iaDetalle.fechaIncorporacion = DateTime.Now;
                        iaDetalle.orden = serial;
                        iaDetalle.TotalPaginas = GlobalClass.GetNumber(r.pag_fin.ToString()) - GlobalClass.GetNumber(r.pag_ini.ToString()) + 1;
                        iaDetalle.PaginaFin = paginas + iaDetalle.TotalPaginas - 1;
                        iaDetalle.PaginaInicio = paginas;
                        iaDetalle.Formato = "PDF";
                        iaDetalle.Origen = "Digitalizado";
                        iaDetalle.Observaciones = r.observacion?.Trim();

                        //SI NO EXISTE EL ARCHIVO O DEBE SOBREESCRIBIRSE LO EXPORTA
                        if (!System.IO.File.Exists(rutaCompleta) || sobreescribir)
                        {
                            PdfDocument doc = new PdfDocument();
                             doc.ImportPageRange(oldPdfDoc, pagIni, pagFin);
                            
                            if (GlobalClass.nom_proyecto.Trim().ToUpper().Contains("RAMA"))
                            {
                                //Si es Rama enumera las páginas
                                for (int i = 0; i < doc.Pages.Count; i++)
                                {
                                    var page = doc.Pages[i];
                                    PdfGraphics graphics = page.Graphics;
                                    PdfGraphicsState state = graphics.Save();

                                    graphics.DrawString((i + iaDetalle.PaginaInicio).ToString(),
                                        new PdfStandardFont(PdfFontFamily.Helvetica, 11),
                                        PdfPens.Black, PdfBrushes.Black,
                                        new PointF((page.Size.Width / 2) - (15 * ((i + 1).ToString().Length)), page.Size.Height - 25));
                                }
                            }

                            doc.Save(rutaCompleta); //Guarda en la ubicación original
                            doc.Close(true); //Cierra PDF Nuevo
                        }

                        string[] args = new String[255];
                        args[0] = rutaCompleta; //ruta archivo
                        args[1] = "true";         //OCR
                        if (ocr == 0) args[1] = "false";
                        args[2] = "false";         //PDFA
                        args[3] = "false";         //METADATA
                        object my_jsondata = null;

                        if (GlobalClass.nom_proyecto.Trim().ToUpper().Contains("FUD"))
                        {
                            args[2] = "true";         //PDFA
                            args[3] = "true";         //METADATA
                            my_jsondata = new
                            {
                                INDICE_CONTENIDO = nomArchivoTipo,
                                FECHA_INDICE_CONTENIDO = DateTime.Now,
                                DOCUMENTO_FOLIADO = serial.ToString().PadLeft(2, '0'),
                                NOMBRE_DOCUMENTO= nomArchivoTipo,
                                TIPOLOGIA_DOCUMENTAL = nomTipoDoc,
                                FECHA_DOCUMENTO= r.fecha.ToString(),
                                FUNCION_RESUMEN="GSE",
                                ORDEN_DOCUMENTO_EXPEDIENTE = serial.ToString().PadLeft(2, '0'),
                                PAGINA_INICIO= pagIni+1,
                                PAGINA_FIN= pagFin+1,
                                DEPENDENCIA_PRODUCTORA= nomSubDependencia,
                                SERIE_DOCUMENTAL=nomSubSerie,
                                FORMATO ="PDFA",
                                ORIGEN="DIGITALIZADO",
                                RESOLUCION_DPI = "300",
                                EMPRESA_DIGITALIZADORA = "IMPRETICS",
                                SOFTWARE_DIGITALIZADOR = "SCANDALL PRO"
                            };
                        }

                        //Tranform it to Json object
                        string json_data = JsonConvert.SerializeObject(my_jsondata); Console.WriteLine(json_data);
                        args[4] = json_data.Replace("\n", "").Replace("\r", "").Replace("\"", "'");

                        string dirAPP = AppDomain.CurrentDomain.BaseDirectory;  //@"C:\Users\USUARIO\Documents\Visual Studio 2019\gdFiles\consolaGDF\cmdGDF\cmdGDF\bin\Debug\cmdGDF.exe"
                        string ejecutable = dirAPP + @"cmdOCR\cmdOCR.exe";
                        if (System.IO.File.Exists(ejecutable))
                        {
                            Process pro = new Process();
                            pro.StartInfo.FileName = ejecutable;
                            pro.StartInfo.Arguments = " \"" + args[0] + "\" \"" + args[1] + "\" \"" + args[2] + "\" \"" + args[3] + "\" \"" + args[4] + "\"";
                            pro.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            var resPro = pro.Start();
                            pro.WaitForExit();
                        }
                        

                        //SI ES RAMA ACTUALIZA EL PSE LUEGO DE OCR
                        if (GlobalClass.nom_proyecto.Trim().ToUpper().Contains("RAMA"))
                        {
                            System.Threading.Thread.Sleep(100);
                            System.IO.FileInfo fi = new System.IO.FileInfo($@"{rutaFinal}{nomArchivoTipo}");
                            iaDetalle.tamanio = fi.Length;
                            ListDetalle.Add(iaDetalle);
                            paginas = iaDetalle.PaginaFin;
                        }

                        //Si el archivo quedó bien guardado actualiza la base de datos t_documento.ruta_docpdf
                        if (System.IO.File.Exists(rutaCompleta))
                        {
                            var Sql = "update dbo.t_documento set ruta_docpdf = '" + rutaFinal + "',nom_docpdf = '" + nomArchivoTipo + "',nro_doc  = '" + nro_doc + "' where id = " + idDoc;
                            await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync(Sql);
                        }

                        //Si es Rama Judicial
                        
                        serial++;
                    }
                    //EntitiesRepository.Entities.SaveChangesAsync();
                    //Si no es proyecto de la RAMA genera Hoja de control
                    if (!GlobalClass.nom_proyecto.Trim().ToUpper().Contains("RAMA") && hc) {
                        p_formato mipFormato = EntitiesRepository.Entities.p_formato.AsNoTracking().Where(f => f.id_proyecto == GlobalClass.id_proyecto).FirstOrDefault();
                        if (mipFormato.ver_hc == 2)
                        {
                            _exportService.pdfHojaControl2_ok(idCarpeta, rutaFinal, false, mipFormato, false);
                        }
                    }
                    //Si es RAMA genera índice
                    if (GlobalClass.nom_proyecto.Trim().ToUpper().Contains("RAMA"))
                    {
                        //Revisa si hay CD's
                        string[] folders = System.IO.Directory.GetDirectories($@"{folderPDF}", "*", System.IO.SearchOption.TopDirectoryOnly);
                        foreach (string folder in folders)
                        {
                            var dirName = new System.IO.DirectoryInfo(folder).Name;
                            if (dirName.Trim().ToUpper().Contains("CD"))
                            {
                                string[] txtCD = dirName.Split('_');

                                //Calcual tamaño Folder
                                long totalByteSize = 0;
                                Alphaleonis.Win32.Filesystem.DirectoryInfo dir = new Alphaleonis.Win32.Filesystem.DirectoryInfo(folder);
                                Alphaleonis.Win32.Filesystem.FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                                foreach (var archivo in files)
                                {
                                    string NombreFull = archivo.FullName;
                                    int lonArchivo = NombreFull.Length;

                                    int i = 0;
                                    while (i < 3 && lonArchivo > 255)
                                    {
                                        string nuevoNombre = string.Empty;
                                        Alphaleonis.Win32.Filesystem.FileInfo anteriorArchivo = new Alphaleonis.Win32.Filesystem.FileInfo(NombreFull);
                                        int diferencia = lonArchivo - 255;
                                        string anteriorNombre = anteriorArchivo.Name;
                                        if (anteriorNombre.Length > diferencia)
                                        {
                                            nuevoNombre = anteriorArchivo.DirectoryName + @"/" + anteriorNombre.Remove(anteriorNombre.Length - diferencia) + anteriorArchivo.Extension;
                                            try
                                            {
                                                Alphaleonis.Win32.Filesystem.File.Move(anteriorArchivo.FullName, nuevoNombre);
                                            }
                                            catch (Exception e)
                                            {
                                                Alphaleonis.Win32.Filesystem.File.Delete(archivo.FullName);
                                            }
                                            
                                        }
                                        NombreFull = nuevoNombre;
                                        lonArchivo = NombreFull.Length;
                                        i++;
                                    }
                                    totalByteSize += archivo.Length;
                                }
                                if (!System.IO.File.Exists($@"{rutaFinal}{dirName}.zip"))
                                {
                                    //Comprime Folder
                                    ZipFile.CreateFromDirectory(folder, $@"{rutaFinal}{dirName}.zip");
                                }


                                //Añadir registro al Indice
                                paginas++;
                                Alphaleonis.Win32.Filesystem.FileInfo fi = new Alphaleonis.Win32.Filesystem.FileInfo($@"{rutaFinal}{dirName}.zip");
                                IndiceArchivoDetalle iaDetalle = new IndiceArchivoDetalle();
                                if(txtCD.Length > 1) iaDetalle.NombreDocumento = txtCD[0] + "_" + txtCD[1];
                                if (txtCD.Length == 1) iaDetalle.NombreDocumento = txtCD[0];
                                iaDetalle.fechaCreacion = DateTime.Now;
                                iaDetalle.fechaIncorporacion = fi.LastWriteTime;
                                iaDetalle.orden = serial;
                                iaDetalle.TotalPaginas = 1;
                                iaDetalle.PaginaFin = paginas + iaDetalle.TotalPaginas - 1;
                                iaDetalle.PaginaInicio = paginas;
                                iaDetalle.Formato = "ZIP";
                                iaDetalle.tamanio = totalByteSize;
                                iaDetalle.Origen = "Electrónico";
                                if(txtCD.Length>2) iaDetalle.Observaciones = txtCD[2];
                                ListDetalle.Add(iaDetalle);
                                paginas = iaDetalle.PaginaFin;
                                serial++;
                            }
                                
                        }
                        if(!string.IsNullOrEmpty(rutaFinal))
                        _exportService.Indice1(iaMaestro, ListDetalle, rutaFinal);
                        //ACTUALIZA TOTAL FOLIOS
                        IndiceArchivoDetalle IAD = ListDetalle.LastOrDefault();
                        var Sql = "update dbo.t_carpeta set total_folios = '" + IAD.PaginaFin.ToString() + "' where id = " + idCarpeta;
                        await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync(Sql);
                    }
                }
            }
        }

        private string getTerceros(int id_carpeta)
        {
            string principal = string.Empty, secundario = string.Empty;
            // Busca terceros en datos básicos
            var terceros = (from c in EntitiesRepository.Entities.t_carpeta
                            join d in EntitiesRepository.Entities.t_documento          on c.id equals d.id_carpeta
                            join dt in EntitiesRepository.Entities.t_documento_tercero on d.id equals dt.id_documento
                            join p in EntitiesRepository.Entities.t_tercero on dt.id_tercero equals p.id
                            where c.id == id_carpeta
                            select new
                            {
                                sol_principal = dt.sol_principal,
                                nombres = p.nombres,
                                apellidos = p.apellidos
                            }).ToList();

                foreach (var item in terceros)
                {
                    if (item.sol_principal)
                    {
                        if(String.IsNullOrEmpty(principal)) principal = $@"{item.nombres} {item.apellidos}";
                        else principal += $@" - {item.nombres} {item.apellidos}";
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(secundario)) secundario = $@"{item.nombres} {item.apellidos}";
                        else secundario += $@" - {item.nombres} {item.apellidos}";
                    }
                }

            return principal + "|" + secundario;
        }

    }
}
