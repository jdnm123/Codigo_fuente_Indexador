using Newtonsoft.Json;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indexai.Helpers
{
    public class LimitedQueue<T> : Queue<T>
    {
        public int Limit { get; set; } = 5;

        public LimitedQueue() : base(5) { }

        public LimitedQueue(int limit) : base(limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(item);
        }
    }

    internal class CacheDocument
    {
        /// <summary>
        /// Nombre del archivo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Key del nombre completo del directory.
        /// </summary>
        public string FullPathKey { get; set; }

        /// <summary>
        /// Dirección en disco del documento.
        /// </summary>
        public string DiskPath { get; set; }
    }

    /// <summary>
    /// Controla el cache de documentos PDF.
    /// </summary>
    public static class CacheHelper
    {
        private static string _cacheDirectory;
        private static string CacheFile = "pdf-cache.json";
        private static LimitedQueue<CacheDocument> _limitedCache;

        /// <summary>
        /// Indica el directorio del cache.
        /// </summary>
        /// <param name="cacheDirectory">Directorio del cache.</param>
        public static void SetCacheDirectory(string cacheDirectory)
        {
            if (!Directory.Exists(cacheDirectory)) Directory.CreateDirectory(cacheDirectory);
            _cacheDirectory = cacheDirectory;
            LoadCache();
        }
        public static void DeleteCache()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"¿Desea Eliminar las imágenes localmente (Cache)?", "Alpha AI", MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;

            List<string>  _files = Directory.EnumerateFiles(_cacheDirectory, "*.pdf", SearchOption.AllDirectories).Reverse().ToList();

            foreach (string archivoCache in _files)
            {
                FileInfo fi = new FileInfo(archivoCache);
                fi.Delete();
            }
            FileInfo fi2 = new FileInfo(CacheFile);
            fi2.Delete();
            _limitedCache = new LimitedQueue<CacheDocument>();
            File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_limitedCache), Encoding.UTF8);
        }
        /// <summary>
        /// Guarda en disco el documento usando la ruta del cache.
        /// </summary>
        /// <param name="pdfLoadedDocument">Documento a guardar.</param>
        /// <param name="filePath">Directorio del documento cargado.</param>
        /// <returns></returns>
        public static void SaveToCacheAsync(PdfViewerControl pdfViewerCrl,  System.Windows.Threading.Dispatcher dispatcher)
        {
            string filePath = $"{pdfViewerCrl.DocumentInfo.FilePath}{pdfViewerCrl.DocumentInfo.FileName}";
            LoadCache();
            FileInfo archivoNuevo = new FileInfo(filePath);
            long tamanioNuevo = archivoNuevo.Length;

            string diskPath = $"{_cacheDirectory}/{archivoNuevo.Name}";

            //Si hay un archivo del mismo tamaño lo borra y escribe el nuevo
            List<string> _files;
            _files = Directory.EnumerateFiles(_cacheDirectory, "*.pdf", SearchOption.AllDirectories).Reverse().ToList();

            foreach (string archivoCache in _files)
            {
                FileInfo fi = new FileInfo(archivoCache);
                long size = fi.Length;
                float tolerancia = 0.05f; //10%
                long tamanioMin = (long)(tamanioNuevo * (1 - tolerancia));
                long tamanioMax = (long)(tamanioNuevo * (1 + tolerancia));

                if (archivoNuevo.Name != fi.Name && size == tamanioNuevo) {
                    fi.Delete();
                } 
                if(archivoNuevo.Name == fi.Name && !(size >= tamanioMin && size <= tamanioMax)){
                     fi.Delete();
                }
            }

            var path = _limitedCache.ToList().FirstOrDefault(x => x.DiskPath == diskPath);
            if (path == null || !File.Exists(diskPath) || (File.Exists(diskPath) &&  File.GetLastWriteTime(filePath) != File.GetLastWriteTime(diskPath)))
            {
                _limitedCache.Enqueue(new CacheDocument
                {
                    DiskPath = diskPath,
                    FullPathKey = filePath,
                    Name = new FileInfo(filePath).Name
                });
                PdfLoadedDocument pdfLoadedDocument = pdfViewerCrl.LoadedDocument;
                dispatcher.Invoke(() => pdfLoadedDocument.Save(diskPath));
                //File.Copy(filePath, diskPath,true);
                //Actualiza fecha de modificación para que sea igual en el servidor y en cache
                DateTime D2 = File.GetLastWriteTime(filePath);
                File.SetLastWriteTime(diskPath, D2);
                File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_limitedCache), Encoding.UTF8);
            }

        }

        /// <summary>
        /// Carga el cache.
        /// </summary>
        private static void LoadCache()
        {
            if (_limitedCache == null)
            {
                if (!File.Exists(CacheFile))
                {
                    _limitedCache = new LimitedQueue<CacheDocument>(5);
                }
                else
                {

                   var list = JsonConvert.DeserializeObject<List<CacheDocument>>(
                        File.ReadAllText(CacheFile, Encoding.UTF8));
                    _limitedCache = new LimitedQueue<CacheDocument>(5);
                    foreach (var item in list)
                    {
                        _limitedCache.Enqueue(item);
                    }
                }
            }
        }

        /// <summary>
        /// Carga un documento desde el cache.
        /// </summary>
        /// <param name="documentPath">Directorio remoto a buscar en el cache.</param>
        /// <returns>Ruta del documento en cache.</returns>
        public static string LoadFromCache(string documentPath)
        {
            string documentPathStandar = documentPath.Replace("/", "\\");
            string RutaFinal = string.Empty;
            LoadCache();
            var cacheDocument = _limitedCache.FirstOrDefault(x => x.FullPathKey == documentPath || x.FullPathKey == documentPathStandar);
            //&& new FileInfo(documentPath).Length == new FileInfo(cacheDocument.DiskPath).Length
            if (cacheDocument == null) return string.Empty;

            RutaFinal = cacheDocument.DiskPath;
            if (!File.Exists(RutaFinal)) return string.Empty;
            if (File.Exists(cacheDocument.FullPathKey) && File.Exists(cacheDocument.DiskPath) && File.GetLastWriteTime(cacheDocument.FullPathKey) != File.GetLastWriteTime(cacheDocument.DiskPath)) return string.Empty;

            bool archivOK = true;
            using (FileStream pdfStream = new FileStream(RutaFinal, FileMode.Open, FileAccess.Read))
            {
                //Create a new instance of PDF document syntax analyzer.
                PdfDocumentAnalyzer analyzer = new PdfDocumentAnalyzer(pdfStream);
                //Analyze the syntax and return the results.
                SyntaxAnalyzerResult analyzerResult = analyzer.AnalyzeSyntax();

                //Check whether the document is corrupted or not.
                if (analyzerResult.IsCorrupted)
                {
                    RutaFinal = string.Empty;
                    archivOK = false;
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.AppendLine("The PDF document is corrupted.");
                    int count = 1;
                    foreach (Syncfusion.Pdf.PdfException exception in analyzerResult.Errors)
                    {
                        strBuilder.AppendLine(count++.ToString() + ": " + exception.Message);
                    }
                    Console.WriteLine(strBuilder);
                }
                else
                {
                    Console.WriteLine("No syntax error found in the provided PDF document");
                }
                analyzer.Close();
            }
            if (!archivOK)
            {
                File.Delete(cacheDocument.DiskPath);
            }
           
            return RutaFinal;
        }

        internal static void SaveToCacheAsync(string ruta)
        {
            if (!File.Exists(ruta)) return;
            string filePath = ruta;
            LoadCache();
            FileInfo archivoNuevo = new FileInfo(filePath);
            long tamanioNuevo = archivoNuevo.Length;

            string diskPath = $"{_cacheDirectory}/{archivoNuevo.Name}";

            //Si hay un archivo del mismo tamaño lo borra y escribe el nuevo
            List<string> _files;
            _files = Directory.EnumerateFiles(_cacheDirectory, "*.pdf", SearchOption.AllDirectories).Reverse().ToList();

            foreach (string archivoCache in _files)
            {
                FileInfo fi = new FileInfo(archivoCache);
                long size = fi.Length;
                float tolerancia = 0.05f; //10%
                long tamanioMin = (long)(tamanioNuevo * (1 - tolerancia));
                long tamanioMax = (long)(tamanioNuevo * (1 + tolerancia));

                if (archivoNuevo.Name != fi.Name && size == tamanioNuevo)
                {
                    fi.Delete();
                }
                if (archivoNuevo.Name == fi.Name && !(size >= tamanioMin && size <= tamanioMax))
                {
                    fi.Delete();
                }
            }

            var path = _limitedCache.ToList().FirstOrDefault(x => x.DiskPath == diskPath);
            if (path == null || !File.Exists(diskPath) || (File.Exists(diskPath) && File.GetLastWriteTime(filePath) != File.GetLastWriteTime(diskPath)))
            {
                _limitedCache.Enqueue(new CacheDocument
                {
                    DiskPath = diskPath,
                    FullPathKey = filePath,
                    Name = new FileInfo(filePath).Name
                });
                File.Copy(ruta, diskPath);
                //File.Copy(filePath, diskPath,true);
                //Actualiza fecha de modificación para que sea igual en el servidor y en cache
                DateTime D2 = File.GetLastWriteTime(filePath);
                File.SetLastWriteTime(diskPath, D2);
                File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_limitedCache), Encoding.UTF8);
            }
        }
    }
}
