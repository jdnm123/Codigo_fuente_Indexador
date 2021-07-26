using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexai.Services
{
    public class regHC2
    {
        public bool nuevo { get; set; }
        public string item { get; set; }
        public string folios { get; set; }
        public string tipoDocumental { get; set; }
        public string desde { get; set; }
        public string hasta { get; set; }
        public string fecha { get; set; }
        public string archivado { get; set; }
        public string fechaIngreso { get; set; }
        public string observaciones { get; set; }

        public static implicit operator List<object>(regHC2 v)
        {
            throw new NotImplementedException();
        }
    }
}
