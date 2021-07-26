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
using Z.EntityFramework.Plus;

namespace Indexai.Views
{
    /// <summary>
    /// Lógica de interacción para BeneficiariosAdmin.xaml
    /// </summary>
    public partial class BeneficiariosAdmin : UserControl
    {
        readonly ObservableCollection<Beneficiarios> Beneficiarios = new ObservableCollection<Beneficiarios>();
        private bool _alreadyDown;

        internal void SetParent(AdminBeneficiarioWindow adminBeneficiarioWindow)
        {
            _adminBeneficiarioWindow = adminBeneficiarioWindow;
        }

        private t_documento _selectedDocument;
         
        private AdminBeneficiarioWindow _adminBeneficiarioWindow;
        private Beneficiarios _selectedBeneficiario;

        public BeneficiariosAdmin()
        {
            InitializeComponent();
            KeyDown += IndexLote_KeyDown;
            KeyUp += IndexLote_KeyUp;
            txtNumeroDocumento.KeyUp += TxtNumeroDocumento_KeyUp;
            txtNumeroDocumento.TextChanged += TxtNumeroDocumento_TextChanged;
            gridBeneficiarios.RecordDeleting += GridBeneficiarios_RecordDeleting;
            gridBeneficiarios.SelectionChanged += GridBeneficiarios_SelectionChanged;
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

        private async void TxtNumeroDocumento_TextChanged(object sender, TextChangedEventArgs e)
        {
            await ActualizarComplementaria();
        }

        private async Task ActualizarComplementaria()
        {

            string id = txtNumeroDocumento.Text.Trim();
            int lon = id.Length;
            if (string.IsNullOrEmpty(id) || lon < 7) return;
           
            var tercero = await Task.Run(() =>
            {
                return NamesService.FindId(id);
            });
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

        internal async void SetDcumento(t_documento documento)
        {
            gridBeneficiarios.ItemsSource = Beneficiarios;
            _selectedDocument = documento ?? throw new ArgumentNullException(nameof(documento));
            var beneficiarios = await Task.Run(()=> EntitiesRepository.Entities.t_documento_tercero.Include("t_tercero").AsNoTracking().Where(x => x.id_documento == _selectedDocument.id));
            if (beneficiarios.Count() != 0)
            {
                foreach (var beneficiario in beneficiarios)
                {
                    Beneficiarios.Add(new Beneficiarios
                    {
                        Apellidos = beneficiario.t_tercero.apellidos,
                        Nombre = beneficiario.t_tercero.nombres,
                        TipoDocumento = beneficiario.t_tercero.tipo_documento,
                        GeneratedEntity = beneficiario.t_tercero,
                        NumeroDocumento = beneficiario.t_tercero.identificacion,
                        DocumentoTercero = beneficiario,
                        sol_principal = beneficiario.sol_principal
                    });
                }
            }
        }

        private void GridBeneficiarios_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (gridBeneficiarios.SelectedIndex != -1)
            {
                _selectedBeneficiario = Beneficiarios[gridBeneficiarios.SelectedIndex];
                btnEliminarBeneficiario.IsEnabled = _selectedBeneficiario != null;
                btnNuevoBeneficiario.IsEnabled = true;
                txtApellidos.Text = _selectedBeneficiario.Apellidos;
                txtNombres.Text = _selectedBeneficiario.Nombre;
                txtNumeroDocumento.Text = _selectedBeneficiario.NumeroDocumento;
                
                chkSolicitante.IsChecked = _selectedBeneficiario.DocumentoTercero.sol_principal;
                cbxTipoDocumento.SelectedValue = _selectedBeneficiario.TipoDocumento;
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
        /// Elimina beneficiario.
        /// </summary>
        /// <param name="beneficiario">Beneficiario a eliminar.</param>
        /// <returns>Task completada.</returns>
        private async Task DeleteBeneficiario(Beneficiarios beneficiario)
        {
            var beneficiarioDb = EntitiesRepository.Entities.t_tercero.IncludeOptimized(x=>x.t_documento_tercero).Where(x=>x.id == beneficiario.GeneratedEntity.id).FirstOrDefault();
            System.Collections.Generic.List<t_documento_tercero> documento_terceros = beneficiarioDb.t_documento_tercero.ToList();
            foreach (var documento_tercero in documento_terceros)
            {
                EntitiesRepository.Entities.t_documento_tercero.Remove(documento_tercero);
            }
            EntitiesRepository.Entities.t_tercero.Remove(beneficiarioDb);
            await EntitiesRepository.Entities.SaveChangesAsync();
            Beneficiarios.Remove(beneficiario);
            btnEliminarBeneficiario.IsEnabled = false;
            CleanInputs();
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

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            //evento de focus para todos los controles que permiten dictado
            var _selectedTextBox = (TextBox)sender;
            object parent = _selectedTextBox.Parent;
            if (parent != null && typeof(StackPanel) == parent.GetType())
            {
                try
                {
                    var suggestionsList = (ListBox)((StackPanel)parent).Children[1];
                    StaticDeepSpeech.SetFocus(sender, Dispatcher, suggestionsList);
                }
                catch (Exception)
                {
                    StaticDeepSpeech.SetFocus(sender, Dispatcher);
                }    
            }
            else
            {
                StaticDeepSpeech.SetFocus(sender, Dispatcher); //dispatcher usado para evitar problemas con los threads de DeepSpeech.
            }
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
                if (gridBeneficiarios.SelectedIndex != -1 && Beneficiarios[gridBeneficiarios.SelectedIndex] != null)
                {
                    var beneficiaro = await EntitiesRepository.Entities.t_tercero.FindAsync(Beneficiarios[gridBeneficiarios.SelectedIndex].GeneratedEntity.id);
                    if (beneficiaro != null)
                    {

                        Beneficiarios[gridBeneficiarios.SelectedIndex] = new Beneficiarios()
                        {
                            Nombre = txtNombres.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                            TipoDocumento = (cbxTipoDocumento.SelectedItem as ComboBoxItem).Tag.ToString(),
                            Apellidos = txtApellidos.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                            NumeroDocumento = txtNumeroDocumento.Text.ToUpper().TrimStart(' ').TrimEnd(' '),
                            GeneratedEntity = Beneficiarios[gridBeneficiarios.SelectedIndex].GeneratedEntity,
                            sol_principal = chkSolicitante.IsChecked.Value
                        };
                        beneficiaro.apellidos = Beneficiarios[gridBeneficiarios.SelectedIndex].Apellidos.ToUpper().TrimStart(' ').TrimEnd(' ');
                        beneficiaro.identificacion = Beneficiarios[gridBeneficiarios.SelectedIndex].NumeroDocumento.ToUpper().TrimStart(' ').TrimEnd(' ');
                        beneficiaro.tipo_documento = Beneficiarios[gridBeneficiarios.SelectedIndex].TipoDocumento.ToUpper().TrimStart(' ').TrimEnd(' ');
                        beneficiaro.nombres = Beneficiarios[gridBeneficiarios.SelectedIndex].Nombre;
                        var t_doc = beneficiaro.t_documento_tercero.Where(x => x.id_tercero == beneficiaro.id).First();
                        t_doc.sol_principal = chkSolicitante.IsChecked.Value;
                        await EntitiesRepository.Entities.SaveChangesAsync();
                        Beneficiarios[gridBeneficiarios.SelectedIndex].DocumentoTercero = t_doc;
                        CleanInputs();
                    }
                }
                else
                {
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
                        id_documento = _selectedDocument.id,
                        sol_principal = chkSolicitante.IsChecked.Value
                    };
                    EntitiesRepository.Entities.t_documento_tercero.Add(documentoTercero);
                    await EntitiesRepository.Entities.SaveChangesAsync();
                    beneficiario.DocumentoTercero = documentoTercero;
                    Beneficiarios.Add(beneficiario);
                    CleanInputs();
                    //MessageBox.Show("Debe seleccionar un beneficiario.", "AI");
                }
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
        private void btnNuevoBeneficiario_Click(object sender, RoutedEventArgs e)
        {
            CleanInputs();
            gridBeneficiarios.SelectedIndex = -1;
            btnNuevoBeneficiario.IsEnabled = false;
            btnEliminarBeneficiario.IsEnabled = false;
            btnAñadir.IsEnabled = true;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e) => _adminBeneficiarioWindow.Close();

        private async void btnEliminarBeneficiario_Click(object sender, RoutedEventArgs e)
        {
            btnEliminarBeneficiario.IsEnabled = false;
            if (_selectedBeneficiario != null)
            {
                await DeleteBeneficiario(_selectedBeneficiario);
            }
        }

        private async void txtNumeroDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            await ActualizarComplementaria();
        }
    }
}
