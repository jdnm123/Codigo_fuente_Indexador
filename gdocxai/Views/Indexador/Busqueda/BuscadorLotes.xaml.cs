using Gestion.DAL;
using Indexai.Views;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Indexai
{
    /// <summary>
    /// Interaction logic for BuscadorLotes.xaml
    /// </summary>
    public partial class BuscadorLotes : UserControl
    {
        private ControlCalidadView _controlCalidadView;
        private FormatosView _formatosView;
        private bool _alreadyDown;
        private AsignarCargaView _asignarCargaView;
        private PreIndexadoView _preindexado;
        private ExportarView _exportarView;

        public BuscadorLotes()
        {
            InitializeComponent();
            KeyDown += IndexLote_KeyDown;
            KeyUp += IndexLote_KeyUp;
        }

        private async void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            int iniPag = string.IsNullOrEmpty(txtRangoMin.Text) ? -1 : Convert.ToInt32(txtRangoMin.Text);
            int maxPag = string.IsNullOrEmpty(txtRangoMax.Text) ? -1 : Convert.ToInt32(txtRangoMax.Text);
            if (_controlCalidadView != null)
            {
                _controlCalidadView.SetFilter(txtCodCarpeta_parse.Text, txtExpediente_parse.Text, txtNomLote.Text, txtNumCaja_parse.Text, txtUsuario.Text, txtNroCarpeta.Text, dateIndex: datePckFechaIndexado.Value, iniPag:iniPag, maxPag: maxPag);
            }
            if (_preindexado != null)
            {
                _preindexado.SetFilter(txtCodCarpeta_parse.Text, txtExpediente_parse.Text, txtNomLote.Text, txtNumCaja_parse.Text, txtUsuario.Text, txtNroCarpeta.Text, iniPag: iniPag, maxPag: maxPag);
            }
            else if (_exportarView != null)
            {
                _exportarView.SetFilter(txtCodCarpeta_parse.Text, txtExpediente_parse.Text, txtNomLote.Text, txtNumCaja_parse.Text, txtUsuario.Text, txtNroCarpeta.Text, iniPag: iniPag, maxPag: maxPag, dateIndex: datePckFechaIndexado.Value);
            }

            if (_formatosView != null)
            {
                _formatosView.UpdateView();
            }
            else if (_asignarCargaView != null)
            {
                await _asignarCargaView.UpdateView();
            }
            else if (_asignarCargaView != null)
            {
                await _asignarCargaView.UpdateView();
            }
            else if (_exportarView != null)
            {
                await _exportarView.UpdateView();
            }
            //else if(_procesoOcrPdfa != null)
            //{
            //      _procesoOcrPdfa.UpdateView();
            //}
        }

        internal void SetControlCalidadView(ControlCalidadView controlCalidadView)
        {
            _controlCalidadView = controlCalidadView;
        }

        internal void SetPreindexadoView(PreIndexadoView preindexadoVW)
        {
            _preindexado = preindexadoVW;
        }

        internal void SetCargaView(AsignarCargaView asignarCargaView)
        {
            _asignarCargaView = asignarCargaView;
        }

        /// <summary>
        /// Use el parent view para ejecutar las consultas.
        /// </summary>
        /// <param name="controlCalidadView"></param>
        internal void SetAdmin(ControlCalidadView controlCalidadView)
        {
            if (controlCalidadView is null)
            {
                throw new ArgumentNullException(nameof(controlCalidadView));
            }
            _controlCalidadView = controlCalidadView;
        }

        internal void SetFormatos(ExportarView exportarView)
        {
            _exportarView = exportarView;
        }

        internal void SetFormatos(FormatosView formatosView)
        {
            _formatosView = formatosView;
        }

        private async void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtCodCarpeta_parse.Text = string.Empty;
            txtExpediente_parse.Text = string.Empty;
            txtNomLote.Text = string.Empty;
            txtNumCaja_parse.Text = string.Empty;

            txtRangoMax.Text = string.Empty;
            txtRangoMin.Text = string.Empty;
            txtNroCarpeta.Text = string.Empty;
            txtUsuario.Text = string.Empty;
            datePckFechaIndexado.Value = new DateTime(0001, 1, 1);

            if (_formatosView != null)
            {
                _formatosView.UpdateView();
            }
            if(_asignarCargaView != null)
            {
                await _asignarCargaView.UpdateView();
            }
            if (_controlCalidadView != null)
            {
                _controlCalidadView.RemoveFilter();
            }
            if (_exportarView != null)
            {
                _exportarView.RemoveFilter();
            }
        }

        internal IQueryable<t_carpeta> GetQueryFilter(IQueryable<t_carpeta> query)
        {
            //REVISAR
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(txtCodCarpeta_parse.Text.Trim()))
                {
                    query = query.Where(x => x.nro_expediente.Contains(txtCodCarpeta_parse.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(txtExpediente_parse.Text.Trim()))
                {
                    query = query.Where(x => x.nom_expediente.Contains(txtExpediente_parse.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(txtNomLote.Text.Trim()))
                {
                    query = query.Where(x => x.t_lote.nom_lote == txtNomLote.Text.Trim());
                }
                if (!string.IsNullOrWhiteSpace(txtNumCaja_parse.Text.Trim())) //recordar que la caja tiene que ser igual para que salga en todos los view
                {
                    query = query.Where(x => x.nro_caja.Equals(txtNumCaja_parse.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(txtUsuario.Text.Trim())) //recordar que la caja tiene que ser igual para que salga en todos los view
                {   //p_usuario = id_usuario(Indexó),p_usuario1 = idusr_asignado("Asignado"), p_usuario2 = idusr_control(Control calidad)
                    query = query.Where(x => x.p_usuario.usuario.Equals(txtUsuario.Text.Trim()) || x.p_usuario1.usuario.Equals(txtUsuario.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(txtNroCarpeta.Text))
                {
                    int NoKP = GlobalClass.GetNumber(txtNroCarpeta.Text);
                    query = query.Where(x => x.nro_carpeta == NoKP);
                }
                if (!string.IsNullOrWhiteSpace(txtRangoMin.Text))
                {
                    int min = Convert.ToInt32(txtRangoMin.Text);
                    query = query.Where(x => x.int_caja >= min);
                }
                if (!string.IsNullOrWhiteSpace(txtRangoMax.Text))
                {
                    int max = Convert.ToInt32(txtRangoMax.Text);
                    query = query.Where(x => x.int_caja<=max);
                }
                if (datePckFechaIndexado.Value != new DateTime(0001, 1, 1))
                {
                    var dateIndex = datePckFechaIndexado.Value;
                    var endDay = new DateTime(dateIndex.Value.Year, dateIndex.Value.Month, dateIndex.Value.Day).AddDays(1);
                    var startDate = new DateTime(dateIndex.Value.Year, dateIndex.Value.Month, dateIndex.Value.Day);
                    query = query.Where(x => (x.fecha_indexa >= startDate) && (x.fecha_indexa < endDay));
                }
            });

            return query;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                var _selectedTextBox = (TextBox)sender;
                object parent = _selectedTextBox.Parent;
                if (parent != null && typeof(StackPanel) == parent.GetType())
                {
                    if (((StackPanel)parent).Children.Count >= 2)
                    {
                        UIElement uIElement = ((StackPanel)parent).Children[1];
                        if (uIElement is ListBox)
                        {
                            var suggestionsList = (ListBox)uIElement;
                            StaticDeepSpeech.SetFocus(sender, Dispatcher, suggestionsList);
                        }
                    }
                    else
                    {
                        StaticDeepSpeech.SetFocus(sender, Dispatcher);
                    }
                }
                else
                {
                    StaticDeepSpeech.SetFocus(sender, Dispatcher);
                }
            }
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                var _selectedTextBox = (TextBox)sender;
                object parent = _selectedTextBox.Parent;
                if (parent != null && typeof(StackPanel) == parent.GetType())
                {
                    //if (((StackPanel)parent).Children.Count >= 2)
                    //{
                    //    var suggestionsList = (ListBox)((StackPanel)parent).Children[1];
                    //    if (suggestionsList.Visibility == Visibility.Visible)
                    //    {
                    //        suggestionsList.Visibility = Visibility.Collapsed;
                    //    }
                    //}
                }
            }
        }

        private void IndexLote_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftAlt || e.Key == Key.System) && _alreadyDown)
            {
                _alreadyDown = false;
                Console.WriteLine("Key up ctrl");
                StaticDeepSpeech.StopRecording();
            }
            else if (e.Key == Key.Escape)
            {
                StaticDeepSpeech.CloseAlternatives();
            }
        }

        private void IndexLote_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftAlt || e.Key == Key.System) && !_alreadyDown)
            {
                _alreadyDown = true;
                Console.WriteLine("Key down ctrl");
                StaticDeepSpeech.StartRecording();
            }
        }
    }
}
