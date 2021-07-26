using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Helpers;
using Indexai.Interfaces;
using Indexai.Services;
using Indexai.Views;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Indexai
{
    /// <summary>
    /// Interaction logic for DatosBasicosLoteWindow.xaml
    /// </summary>
    public partial class DatosBasicosLoteWindow : Window, IInputForm
    {
        private IndexadorView _indexador;
        private CarpetaModel _carpetaModel;
        private bool _alreadyDown;
        private ControlCalidadRevisionView _controlCalidadView;
        private ExportarView _adminView;

        public DatosBasicosLoteWindow()
        {
            InitializeComponent();
            txtExpediente.Text = GlobalClass.Carpeta.NoExpediente;
            txtNomExpediente.Text = GlobalClass.Carpeta.nom_expediente;
            txtTotalFolios_numerico.Text = GlobalClass.Carpeta.Folios.ToString();
            txtNumeroCarpeta.Text = GlobalClass.Carpeta.nro_carpeta.ToString();
            txtNumCaja.Text = GlobalClass.Carpeta.nro_caja;
            txthcinio_numerico.Text = GlobalClass.Carpeta.hc_inicio;
            txthcfin_numerico.Text = GlobalClass.Carpeta.hc_fin;
            txtTomoIni.Text = GlobalClass.Carpeta.tomo;
            txtTomoFin.Text = GlobalClass.Carpeta.tomo_fin;
            txtObservaciones.Text = GlobalClass.Carpeta.Observaciones;
            KeyDown += IndexLote_KeyDown;
            KeyUp += IndexLote_KeyUp;
            Loaded += DatosBasicosLoteWindow_Loaded;
            if (string.IsNullOrEmpty(txtTotalFolios_numerico.Text)) txtTotalFolios_numerico.Focus();
            else if (string.IsNullOrEmpty(txtExpediente.Text)) txtTotalFolios_numerico.Focus();
            IsVisibleChanged += DatosBasicosLoteWindow_IsVisibleChanged;
        }

        private void DatosBasicosLoteWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                StaticDeepSpeech.OnComando += StaticDeepSpeech_OnComando;
            }
            else
            {
                StaticDeepSpeech.OnComando -= StaticDeepSpeech_OnComando;
            }
        }

        private void StaticDeepSpeech_OnComando(object sender, ComandoEventArgs e)
        {
            Save();
        }

        private void DatosBasicosLoteWindow_Loaded(object sender, RoutedEventArgs e) => Left += 400; //mueve 400 píxeles a la derecha

        private void IndexLote_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftAlt || e.Key == Key.System) && _alreadyDown)
            {
                _alreadyDown = false;
                Console.WriteLine("Key up ctrl");
                StaticDeepSpeech.StopRecording();
            }
            else if ((e.Key == Key.RightShift) && _alreadyDown)
            {
                _alreadyDown = false;
                Console.WriteLine("Key up ctrl");
                StaticDeepSpeech.StopRecording(append:true);
            }
            else if (e.Key == Key.Escape)
            {
                StaticDeepSpeech.CloseAlternatives();
            }
        }

        private void IndexLote_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftAlt || e.Key == Key.System || e.Key == Key.RightShift) && !_alreadyDown)
            {
                _alreadyDown = true;
                Console.WriteLine("Key down ctrl");
                StaticDeepSpeech.StartRecording();
            }
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            //evento de focus para todos los controles que permiten dictado
            var _selectedTextBox = (TextBox)sender;
            object parent = _selectedTextBox.Parent;
            if (parent != null && typeof(StackPanel) == parent.GetType())
            {
                var suggestionsList = (ListBox)((StackPanel)parent).Children[1];
                StaticDeepSpeech.SetFocus(sender, Dispatcher, suggestionsList);
            }
            else
            {
                StaticDeepSpeech.SetFocus(sender, Dispatcher); //dispatcher usado para evitar problemas con los threads de DeepSpeech.
            }
        }

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender; //se require que la ventana siempre sea visible, incluso cuando pierde el focus.
            window.Topmost = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            btnAceptar.IsEnabled = false;
            SaveShanges();
            btnAceptar.IsEnabled = true;
        }

        private async void renombrararchivo(int idLote, string caja, string carpetaOLD, string carpetaNEW)
        {
            if (carpetaOLD != carpetaNEW)
            {
                var lote = EntitiesRepository.Entities.t_lote.AsNoTracking().Where(x => x.id == idLote).FirstOrDefault();
                if (lote != null)
                {
                    string folderPDF = GlobalClass.ruta_proyecto + $@"/{lote.nom_lote}/{caja}/{carpetaOLD}/";
                    if (Directory.Exists(folderPDF))
                    {
                        if (_controlCalidadView != null)
                        {
                            _controlCalidadView._loadedDocument.Close();
                        }
                        string newRutaFolder = GlobalClass.ruta_proyecto + $@"/{lote.nom_lote}/{caja}/{carpetaNEW}/";
                        Directory.Move(folderPDF, newRutaFolder);
                        folderPDF = newRutaFolder;

                    }
                    string rutaOLD = folderPDF + $@"{carpetaOLD}.pdf";
                    if (!File.Exists(rutaOLD)) folderPDF = GlobalClass.ruta_proyecto + $@"/{lote.nom_lote}/{caja}/";
                    rutaOLD = folderPDF + $@"{carpetaOLD}.pdf";
                    string rutaNEW = folderPDF + $@"{carpetaNEW}.pdf";
                    if (File.Exists(rutaOLD))
                    {
                        if (File.Exists(rutaNEW))
                        {
                            MessageBoxResult messageBoxResult = MessageBox.Show($@"¿El archivo {carpetaNEW} ya existe, desea sobreescribirlo?", "Importante", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                string oldFileName = rutaNEW.TrimEnd(".pdf".ToCharArray()) + $"old{Guid.NewGuid()}.pdf";
                                File.Move(rutaNEW, oldFileName);
                                File.Delete(rutaNEW);
                                File.Move(rutaOLD, rutaNEW); //crea copia del archivo que se está editando
                            }
                        }
                        else
                        {
                            File.Move(rutaOLD, rutaNEW); //Mueve el archivo que se está editando
                        }
                    }
                    if (_controlCalidadView != null)
                    {
                        _controlCalidadView.UpdateView();
                    }
                }
            }
        }

        /// <summary>
        /// Valida los campos y guarda los cambios.
        /// </summary>
        public async void SaveShanges()
        {
            string NroExp = string.Empty;
            if (string.IsNullOrEmpty(txtExpediente.Text))
            {
                MessageBox.Show("Debe digitar el número de Carpeta.", "AI");
                txtExpediente.Focus();
                return;
            }
            NroExp = txtExpediente.Text.ToUpper().TrimStart(' ').TrimEnd(' ');
            if (string.IsNullOrEmpty(txtTotalFolios_numerico.Text) ||
                GlobalClass.GetNumber(txtTotalFolios_numerico.Text) < 0)
            {
                MessageBox.Show("Debe digitar el número de Folios.", "AI");
                txtTotalFolios_numerico.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtNomExpediente.Text))
            {
                MessageBox.Show("Debe digitar la descripción.", "AI");
                txtNomExpediente.Focus();
                return;
            }
            if (txtNomExpediente.Text.Length > 1000)
            {
                MessageBox.Show("No puede superar los 1000 caracteres.", "AI");
                txtNomExpediente.Focus();
                return;
            }
            if (txthcinio_numerico.Text.Length > 10)
            {
                MessageBox.Show("El largo de hc inicio no puede ser mayor a 10 caracteres.", "AI");
                txthcinio_numerico.Focus();
                return;
            }
            if (txthcinio_numerico.Text.Length > 0 && GlobalClass.GetNumber(txthcinio_numerico.Text) == 0)
            {
                MessageBox.Show("HC inicio debe ser númerico.", "AI");
                txthcinio_numerico.Focus();
                return;
            }
            if (txthcfin_numerico.Text.Length > 10)
            {
                MessageBox.Show("El largo de hc fin no puede ser mayor a 10 caracteres.", "AI");
                txthcinio_numerico.Focus();
                return;
            }
            if (txthcfin_numerico.Text.Length > 0 && GlobalClass.GetNumber(txthcfin_numerico.Text) == 0)
            {
                MessageBox.Show("HC fin debe ser númerico.", "AI");
                txthcfin_numerico.Focus();
                return;
            }
            if (txtTomoIni.Text.Length > 10)
            {
                MessageBox.Show("El tomo no puede ser mayor a 10 caracteres.", "AI");
                txtTomoIni.Focus();
                return;
            }
            if (txtTomoIni.Text.Length > 0 && GlobalClass.GetNumber(txtTomoIni.Text) == 0)
            {
                MessageBox.Show("HC fin debe ser númerico.", "AI");
                txtTomoIni.Focus();
                return;
            }
            if (txtTomoFin.Text.Length > 10)
            {
                MessageBox.Show("El tomo final no puede ser mayor a 10 caracteres.", "AI");
                txtTomoFin.Focus();
                return;
            }
            if (txtTomoFin.Text.Length > 0 && GlobalClass.GetNumber(txtTomoFin.Text) == 0)
            {
                MessageBox.Show("El tomo final debe ser númerico.", "AI");
                txtTomoFin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtNumeroCarpeta.Text))
            {
                MessageBox.Show("Debe digitar el número carpeta.", "AI");
                return;
            }

            GlobalClass.Carpeta.nom_expediente = txtNomExpediente.Text.ToUpper().TrimStart(' ').TrimEnd(' ');
            string identificacion = "NA";
            int numDescripcion = GlobalClass.GetNumber(txtNomExpediente.Text);
            if (numDescripcion != 0) identificacion = numDescripcion.ToString();
            try
            {
                if (_carpetaModel.Beneficiario != null) //si beneficiario principal existe se carga
                {
                    using (gdocxEntities Entities = new gdocxEntities())
                    {
                        //var beneficiario = await Entities.t_tercero.FindAsync(_carpetaModel.Beneficiario.id);
                        //beneficiario.tipo_tercero = "PERSONA";
                        //beneficiario.identificacion = identificacion.PadLeft(50).TrimStart(' ').TrimEnd(' ');
                        //await Entities.SaveChangesAsync();
                        //Actualiza información de carpeta
                        t_carpeta kp = Entities.t_carpeta.FirstOrDefault(x => x.id == _carpetaModel.IdCarptera);
                        string kpOLD = kp.nro_expediente;
                        kp.nro_expediente = NroExp;
                        kp.total_folios = GlobalClass.GetNumber(txtTotalFolios_numerico.Text);
                        kp.nom_expediente = txtNomExpediente.Text.ToUpper().TrimStart(' ').TrimEnd(' ');
                        kp.hc_inicio = GlobalClass.GetNumber(txthcinio_numerico.Text).ToString();
                        kp.hc_fin = GlobalClass.GetNumber(txthcfin_numerico.Text).ToString();
                        kp.tomo = GlobalClass.GetNumber(txtTomoIni.Text).ToString();
                        kp.tomo_fin = GlobalClass.GetNumber(txtTomoFin.Text).ToString();
                        kp.nro_carpeta = GlobalClass.GetNumber(txtNumeroCarpeta.Text);
                        kp.kp_observacion = txtObservaciones.Text;
                        int result = await Entities.SaveChangesAsync();

                        renombrararchivo(GlobalClass.GetNumber(kp.id_lote.ToString()), kp.nro_caja, kpOLD, NroExp);
                    }
                    //EntitiesRepository.Reset(); // reinicia para cargar los cambios del nuevo contexto
                }
                else
                {
                    var t_carpetaId = _carpetaModel.IdCarptera;
                    //t_tercero newTercero = new t_tercero
                    //{
                    //    tipo_tercero = "PERSONA",
                    //    identificacion = identificacion.PadLeft(50).TrimStart(' ').TrimEnd(' ')
                    //};
                    using (gdocxEntities Entities = new gdocxEntities())
                    {
                        //Actualiza información de carpeta
                        t_carpeta kp = Entities.t_carpeta.FirstOrDefault(x => x.id == t_carpetaId);
                        string kpOLD = kp.nro_expediente;
                        kp.nro_expediente = NroExp;
                        kp.total_folios = GlobalClass.GetNumber(txtTotalFolios_numerico.Text); //caja.nro_caja = txtNumCaja.Text;//caja.nro_expediente = txtExpediente.Text;
                        kp.nom_expediente = txtNomExpediente.Text.ToUpper().TrimStart(' ').TrimEnd(' ');
                        //kp.t_tercero = newTercero;
                        kp.hc_inicio = GlobalClass.GetNumber(txthcinio_numerico.Text).ToString();
                        kp.hc_fin = GlobalClass.GetNumber(txthcfin_numerico.Text).ToString();
                        kp.tomo = GlobalClass.GetNumber(txtTomoIni.Text).ToString();
                        kp.tomo_fin = GlobalClass.GetNumber(txtTomoFin.Text).ToString();
                        kp.nro_carpeta = GlobalClass.GetNumber(txtNumeroCarpeta.Text);
                        kp.kp_observacion = txtObservaciones.Text;
                        int result = await Entities.SaveChangesAsync();
                        if(_indexador != null)  _indexador.btnIndexar.IsEnabled = true; //Independiente del If habilita el botón indexar
                        renombrararchivo(GlobalClass.GetNumber(kp.id_lote.ToString()), kp.nro_caja, kpOLD, NroExp);
                    }
                    //EntitiesRepository.Reset(); // reinicia para cargar los cambios del nuevo contexto
                }
                _indexador?.UpdateFromBasicoLote();

                Close();
                if (_indexador != null) _indexador.btnIndexar.IsEnabled = true;
            }
            catch (DbEntityValidationException ex)
            {
                var newException = new FormattedDbEntityValidationException(ex);
                Telemetry.TrackException(ex);
            }

        }

        /// <summary>
        /// Indica la carptea con el beneficiario principal
        /// </summary>
        /// <param name="indexador">Parent view.</param>
        /// <param name="selectedItem">Carpeta seleccionada.</param>
        internal void SetSelectedCarpeta(IndexadorView indexador, CarpetaModel selectedItem)
        {
            _indexador = indexador;
            SetCarpeta(selectedItem);
        }

        /// <summary>
        /// Indica la carptea con el beneficiario principal
        /// </summary>
        /// <param name="indexador">Parent view.</param>
        /// <param name="selectedItem">Carpeta seleccionada.</param>
        internal void SetSelectedCarpeta(ControlCalidadRevisionView controlCalidadView, CarpetaModel selectedItem)
        {
            _controlCalidadView = controlCalidadView;
            SetCarpeta(selectedItem);
        }

        internal void SetSelectedCarpeta(ExportarView admin, CarpetaModel selectedItem)
        {
            _adminView = admin;
            SetCarpeta(selectedItem);
        }

        private void SetCarpeta(CarpetaModel selectedItem)
        {
            _carpetaModel = selectedItem ?? throw new ArgumentNullException(nameof(selectedItem));
            if (_carpetaModel.Beneficiario != null)
            {
                txthcinio_numerico.Text = _carpetaModel.hc_inicio;
                txthcfin_numerico.Text = _carpetaModel.hc_fin;
                txtTomoIni.Text = _carpetaModel.tomo;
                txtTomoFin.Text = _carpetaModel.tomo_fin;
            }
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            //lógica de sugerencia en los campos para reconocimientos alternos.
            var _selectedTextBox = (TextBox)sender;
            object parent = _selectedTextBox.Parent;
            if (parent != null && typeof(StackPanel) == parent.GetType())
            {
                if (((StackPanel)parent).Children.Count >= 2)
                {
                    var suggestionsList = (ListBox)((StackPanel)parent).Children[1];
                    if (suggestionsList.Visibility == Visibility.Visible)
                    {
                        suggestionsList.Visibility = Visibility.Collapsed;
                    }
                }
            }
            StaticDeepSpeech.StopRecording(true);
        }
    }
}