using Gestion.DAL;
using Indexai.Models;
using NamesServiceLib;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Indexai
{
    /// <summary>
    /// Interaction logic for InformacionComplementaria.xaml
    /// </summary>
    public partial class InformacionComplementaria : Window
    {
        readonly ObservableCollection<Beneficiarios> BeneficiariosColl = new ObservableCollection<Beneficiarios>();
        private t_documento _documento;

        public int SelPagInicial { get; private set; }
        public int SelPagFinal { get; private set; }

        private bool _alreadyDown;
        private Beneficiarios _selectedBeneficiario;

        public InformacionComplementaria()
        {
            InitializeComponent();
            gridBeneficiarios.ItemsSource = BeneficiariosColl;
            KeyDown += IndexLote_KeyDown;
            KeyUp += IndexLote_KeyUp;
            txtNumeroDocumento.KeyUp += TxtNumeroDocumento_KeyUp;
            txtNumeroDocumento.TextChanged += TxtNumeroDocumento_TextChanged;
            gridBeneficiarios.RecordDeleting += GridBeneficiarios_RecordDeleting;
            gridBeneficiarios.SelectionChanged += GridBeneficiarios_SelectionChanged;
            Left = SystemParameters.PrimaryScreenWidth - Width;
            IsVisibleChanged += BeneficiariosAdmin_IsVisibleChanged;
        }

        private void BeneficiariosAdmin_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        private async void StaticDeepSpeech_OnComando(object sender, ComandoEventArgs e)
        {
            await SaveUpdate();
        }

        private void GridBeneficiarios_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (gridBeneficiarios.SelectedIndex != -1)
            {
                _selectedBeneficiario = BeneficiariosColl[gridBeneficiarios.SelectedIndex];
                btnEliminarBeneficiario.IsEnabled = _selectedBeneficiario != null;
                btnNuevoBeneficiario.IsEnabled = true;
                txtApellidos.Text = _selectedBeneficiario.Apellidos;
                txtNombres.Text = _selectedBeneficiario.Nombre;
                txtNumeroDocumento.Text = _selectedBeneficiario.NumeroDocumento;
                chkSolicitante.IsChecked = _selectedBeneficiario.DocumentoTercero.sol_principal;
                cbxTipoDocumento.SelectedValue = _selectedBeneficiario.TipoDocumento.TrimStart(' ').TrimEnd(' ');
                for (int i = 0; i < cbxTipoDocumento.Items.Count; i++)
                {
                    ComboBoxItem item = (ComboBoxItem)cbxTipoDocumento.Items[i];
                    var valTipoDoc = _selectedBeneficiario.TipoDocumento.ToLower().TrimStart(' ').TrimEnd(' ');
                    if (item.Content.ToString().ToLower() == valTipoDoc || item.Tag.ToString().ToLower() == valTipoDoc)
                    {
                        cbxTipoDocumento.SelectedIndex = i;
                    }
                }
            }
            else
            {
                CleanInputs();
            }
        }

        private async void GridBeneficiarios_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea eliminar beneficiario?", "Eliminar beneficiario", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var beneficiario = e.Items[0] as Beneficiarios;
                await DeleteBeneficiario(beneficiario);
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Elimina el beneficiario.
        /// </summary>
        /// <param name="beneficiario">Beneficiario a eliminar.</param>
        /// <returns>Tarea completada.</returns>
        private async Task DeleteBeneficiario(Beneficiarios beneficiario)
        {
            EntitiesRepository.Entities.t_documento_tercero.Remove(beneficiario.DocumentoTercero);
            EntitiesRepository.Entities.t_tercero.Remove(beneficiario.GeneratedEntity);
            await EntitiesRepository.Entities.SaveChangesAsync();
            BeneficiariosColl.Remove(beneficiario);
            btnEliminarBeneficiario.IsEnabled = false;
            CleanInputs();
            gridBeneficiarios.GridColumnSizer.Refresh();
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            var _selectedTextBox = (TextBox)sender;
            object parent = _selectedTextBox.Parent;
            if (parent != null && typeof(StackPanel) == parent.GetType())
            {
                if (((StackPanel)parent).Children.Count >= 2)
                {
                    var suggestionsList = (ListBox)((StackPanel)parent).Children[1];
                    StaticDeepSpeech.SetFocus(sender, Dispatcher, suggestionsList);
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
                StaticDeepSpeech.StopRecording(append: true);
            }
            else if (e.Key == Key.Escape)
            {
                StaticDeepSpeech.CloseAlternatives();
            }
        }
        private async void TxtNumeroDocumento_TextChanged(object sender, TextChangedEventArgs e)
        {
            await ActualizarComplementaria();
        }

        private async Task ActualizarComplementaria()
        {
            string id = txtNumeroDocumento.Text.Trim();
            int lon = id.Length;
            if (string.IsNullOrEmpty(id) || lon < 7) return;

            var tercero = await Task.Run(() => NamesService.FindId(id));
            if (tercero != null)
            {
                txtNombres.Text = tercero.Nombre;
                txtApellidos.Text = tercero.Apelllidos;
            }
        }

        private void TxtNumeroDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tercero = NamesService.FindId(txtNumeroDocumento.Text);
                if (tercero != null)
                {
                    txtNombres.Text = tercero.Nombre;
                    txtApellidos.Text = tercero.Apelllidos;
                }
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
        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender; //se requiere que la ventana siempre sea visible.
            window.Topmost = true;
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
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
        }

        private async void btnAñadir_Click(object sender, RoutedEventArgs e)
        {
            await SaveUpdate();
        }

        private async Task SaveUpdate()
        {
            btnAñadir.IsEnabled = false;
            try
            {
                if (cbxTipoDocumento.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar el tipo de documento", "AI");
                    cbxTipoDocumento.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtNombres.Text))
                {
                    MessageBox.Show("Debe digitar el Nombre", "AI");
                    txtNombres.Focus();
                    btnAñadir.IsEnabled = true;
                    return;
                }
                if (string.IsNullOrEmpty(txtApellidos.Text))
                {
                    MessageBox.Show("Debe digitar el Apellido", "AI");
                    txtApellidos.Focus();
                    btnAñadir.IsEnabled = true;
                    return;
                }
                if (gridBeneficiarios.SelectedIndex != -1 && BeneficiariosColl[gridBeneficiarios.SelectedIndex] != null)
                {
                    var beneficiaro = await EntitiesRepository.Entities.t_tercero.FindAsync(BeneficiariosColl[gridBeneficiarios.SelectedIndex].GeneratedEntity.id);
                    if (beneficiaro != null)
                    {

                        BeneficiariosColl[gridBeneficiarios.SelectedIndex] = new Beneficiarios()
                        {
                            Nombre = txtNombres.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                            TipoDocumento = (cbxTipoDocumento.SelectedItem as ComboBoxItem).Tag.ToString(),
                            Apellidos = txtApellidos.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                            NumeroDocumento = txtNumeroDocumento.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                            GeneratedEntity = BeneficiariosColl[gridBeneficiarios.SelectedIndex].GeneratedEntity,
                            sol_principal = chkSolicitante.IsChecked.Value
                        };
                        beneficiaro.apellidos = BeneficiariosColl[gridBeneficiarios.SelectedIndex].Apellidos.ToUpper().TrimStart(' ').TrimEnd(' ');
                        beneficiaro.identificacion = BeneficiariosColl[gridBeneficiarios.SelectedIndex].NumeroDocumento.ToUpper().TrimStart(' ').TrimEnd(' ');
                        beneficiaro.tipo_documento = BeneficiariosColl[gridBeneficiarios.SelectedIndex].TipoDocumento.ToUpper().TrimStart(' ').TrimEnd(' ');
                        beneficiaro.nombres = BeneficiariosColl[gridBeneficiarios.SelectedIndex].Nombre;
                        var t_doc = beneficiaro.t_documento_tercero.Where(x => x.id_tercero == BeneficiariosColl[gridBeneficiarios.SelectedIndex].GeneratedEntity.id).First();
                        t_doc.sol_principal = chkSolicitante.IsChecked.Value;

                        await EntitiesRepository.Entities.SaveChangesAsync();
                        BeneficiariosColl[gridBeneficiarios.SelectedIndex].DocumentoTercero = t_doc;
                        CleanInputs();
                    }
                }
                else
                {
                    //if (string.IsNullOrEmpty(txtNumeroDocumento.Text))
                    //{
                    //    MessageBox.Show("debe digitar la identificación", "AI");
                    //    txtNumeroDocumento.Focus();
                    //    return;
                    //}
                    var beneficiario = new Beneficiarios()
                    {
                        Nombre = txtNombres.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                        TipoDocumento = (cbxTipoDocumento.SelectedItem as ComboBoxItem).Tag.ToString(),
                        Apellidos = txtApellidos.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                        NumeroDocumento = txtNumeroDocumento.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                        sol_principal = chkSolicitante.IsChecked.Value
                    };

                    t_tercero beneficiarioDb = new t_tercero
                    {
                        apellidos = beneficiario.Apellidos.ToUpper().TrimStart(' ').TrimEnd(' '),
                        identificacion = beneficiario.NumeroDocumento.ToUpper().TrimStart(' ').TrimEnd(' '),
                        tipo_documento = beneficiario.TipoDocumento.ToUpper().TrimStart(' ').TrimEnd(' '),
                        nombres = beneficiario.Nombre,
                        tipo_tercero = "PERSONA"
                    };
                    EntitiesRepository.Entities.t_tercero.Add(beneficiarioDb);
                    await EntitiesRepository.Entities.SaveChangesAsync();
                    beneficiario.GeneratedEntity = beneficiarioDb;
                    t_documento_tercero documentoTercero = new t_documento_tercero
                    {
                        id_tercero = beneficiarioDb.id,
                        id_documento = _documento.id,
                        sol_principal = chkSolicitante.IsChecked.Value
                    };
                    EntitiesRepository.Entities.t_documento_tercero.Add(documentoTercero);
                    await EntitiesRepository.Entities.SaveChangesAsync();
                    beneficiario.DocumentoTercero = documentoTercero;
                    BeneficiariosColl.Add(beneficiario);
                    CleanInputs();
                }
                gridBeneficiarios.GridColumnSizer.Refresh();
                txtNumeroDocumento.Focus();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                btnAñadir.IsEnabled = true;
            }
        }

        private void CleanInputs()
        {
            txtNumeroDocumento.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            chkSolicitante.IsChecked = false;
            gridBeneficiarios.SelectedIndex = -1;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e) => Close();

        internal void SetDocumento(t_documento documento, int selPagInicial, int selPagFinal)
        {
            _documento = documento ?? throw new ArgumentNullException(nameof(documento));
            SelPagInicial = selPagInicial;
            SelPagFinal = selPagFinal;
        }

        private async void btnEliminarBeneficiario_Click(object sender, RoutedEventArgs e)
        {
            await DeleteBeneficiario(_selectedBeneficiario);
        }

        private void btnNuevoBeneficiario_Click(object sender, RoutedEventArgs e)
        {
            CleanInputs();
            gridBeneficiarios.SelectedIndex = -1;
            btnNuevoBeneficiario.IsEnabled = false;
            btnEliminarBeneficiario.IsEnabled = false;
            btnAñadir.IsEnabled = true;
        }

        private async void txtNumeroDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            await ActualizarComplementaria();
        }
    }
}
