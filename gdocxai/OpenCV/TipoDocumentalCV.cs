using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;

namespace Indexai.OpenCV
{
    public class TipoDocumentalCV
    {
        public string Id { get; set; }
        public List<Image<Gray, byte>> Images { get; set; }
        public int Count { get; set; }
    }
}
