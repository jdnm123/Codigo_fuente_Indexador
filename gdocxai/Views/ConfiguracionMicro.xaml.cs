using CSCore.CoreAudioAPI;
using Indexai.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Indexai
{
    /// <summary>
    /// Interaction logic for ConfiguracionMicro.xaml
    /// </summary>
    public partial class ConfiguracionMicro : Window
    {
        private const string ConfigurationFile = "micro-config.json";
        private MicroConfig _configuration;

        public ConfiguracionMicro()
        {
            InitializeComponent();
            LoadDevices();

            cbxSelectedMicro.ItemsSource = AvailableRecordDevices;
            cbxSelectedMicro.SelectedItem = SelectedDevice;

            if (AvailableRecordDevices?.Count != 0)
                SelectedDevice = AvailableRecordDevices[0];
            cbxSelectedMicro.SelectionChanged += CbxSelectedMicro_SelectionChanged;
            if (File.Exists(ConfigurationFile))
            {
                //carga la configuración del micrófono de un archivo JSON
                _configuration = JsonConvert.DeserializeObject<MicroConfig>(File.ReadAllText(ConfigurationFile, Encoding.UTF8));
                if (AvailableRecordDevices.Count(x => x.FriendlyName == _configuration.SelectedMicrophone) != 0)
                {
                    cbxSelectedMicro.SelectedIndex = AvailableRecordDevices.ToList().FindIndex(x => x.FriendlyName == _configuration.SelectedMicrophone);
                }
            }
            else
            {
                _configuration = new MicroConfig();
            }
            Closing += ConfiguracionMicro_Closing;
        }

        private void ConfiguracionMicro_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (chkShow.IsChecked.Value)
            {
                if (_configuration == null)
                {
                    _configuration = new MicroConfig();
                }
                _configuration.Show = !chkShow.IsChecked.Value;
                SaveConfig();
            }
        }

        /// <summary>
        /// Cargar los dispositicov de grabación disponibles.
        /// </summary>
        private void LoadDevices()
        {
            _devicesList = MMDeviceEnumerator.EnumerateDevices(DataFlow.Capture, DeviceState.Active);
            AvailableRecordDevices = new ObservableCollection<MMDevice>(
                _devicesList);
            if (_devicesList.Count == 0)
            {
                //MessageBox.Show("No se han detectado dispositivos de grabación.");
                //Close();
            }
        }

        private void CbxSelectedMicro_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedDevice = AvailableRecordDevices[cbxSelectedMicro.SelectedIndex];
        }

        private MMDeviceCollection _devicesList;

        public ObservableCollection<MMDevice> AvailableRecordDevices { get; set; }
        public MMDevice SelectedDevice { get; set; }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            UpdateMicro();
        }

        /// <summary>
        /// Actualiza la entrada seleccionada y guarda en JSON.
        /// </summary>
        private void UpdateMicro()
        {
            if(SelectedDevice != null)
            {
                lblResult.Content = "Configuración guardada";
                _configuration.SelectedMicrophone = SelectedDevice.FriendlyName;
                SaveConfig();
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Guarda la configuración en un achivo JSON.
        /// </summary>
        private void SaveConfig()
        {
            _configuration.Show = !chkShow.IsChecked.Value;
            string contents = JsonConvert.SerializeObject(_configuration);
            File.WriteAllText(ConfigurationFile, contents, Encoding.UTF8);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender; //se require que la ventana siempre sea visible, incluso cuando pierde el focus.
            window.Topmost = true;
        }
    }
}