namespace Indexai.Models
{
    public class MicroConfig
    {
        /// <summary>
        /// Micrófono seleccionado.
        /// </summary>
        public string SelectedMicrophone { get; set; }

        /// <summary>
        /// Indica si la ventana de selección de entrada de audio se debe mostrar.
        /// </summary>
        public bool Show { get; set; }

        public MicroConfig()
        {

        }
    }
}
