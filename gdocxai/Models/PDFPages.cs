using System.Windows.Media.Imaging;

namespace Indexai.Models
{
    /// <summary>
    /// Modelo de las imágenes del listview.
    /// </summary>
    public class PDFPages
    {
        private BitmapSource _imageData;
        private bool _edited = false; //por defecto la imagen inicia sin edición.
        public string Index { get; set; }
        public string IndexOld { get; set; }
        public BitmapSource ImageData
        {
            get => _imageData; set
            {
                _imageData = value;
            }
        }
        /// <summary>
        /// Indica si la imagen fue editada.
        /// </summary>
        public bool Edited { get => _edited; set => _edited = value; }
        public BitmapSource Source { get; internal set; }

        //ToImageSource
        /*internal void Dispose()
        {
            Source = null;
            ImageData = null;
            System.GC.Collect();
        } */
    }
}