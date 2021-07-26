using Gestion.DAL;
using Indexai.Helpers;
using Indexai.Models;
using Indexai.Services;
using Indexai.Views;
using Syncfusion.Windows.Controls.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Indexai
{

    class SeleccionItem
    {
        public string Nombre { get; set; }
        public string NombreLimpio { get; set; }
        public string Valor { get; set; }
    }

    /// <summary>
    /// Interaction logic for IndexarDocumentos.xaml
    /// </summary>
    public partial class IndexarDocumentos : UserControl
    {
        /// <summary>
        /// Tipos de documento para el combobox.
        /// </summary>
        private ObservableCollection<p_tipodoc> _tiposDoc = new ObservableCollection<p_tipodoc>();

        private p_tipodoc _selectedTipoDoc;
        private List<p_tipoitem> _tipoItem;
        private IndexadorView _indexadorView;
        private t_documento _documento;
        private InformacionComplementaria _informacionComplementaria;
        private bool _alreadyDown;
        private int _contadorIndexados = 0;

        internal void SetAdmin(bool isAdmin) => _isAdmin = isAdmin;

        /// <summary>
        /// Indica si el usuario actual es aministrador
        /// </summary>
        private bool _isAdmin;

        private bool _adminEdit;
        private string _invalidFormMesssage;

        /// <summary>
        /// Carga los elementos al view dinámico si es la primera carga
        /// </summary>
        private ControlCalidadIListItem _selectedDocumentAdmin;

        private string _firstAdminSelectionIndex;
        private ControlCalidadRevisionView _controlCalidadView;
        private Dictionary<string, string> _viewValues = new Dictionary<string, string>();
        private bool _indexadoView;

        public IndexarDocumentos()
        {
            InitializeComponent();
            Loaded += IndexarDocumentos_Loaded;
            //cbxArchivadores.ItemsSource = _tiposDoc;

            tipoDocumentos.SelectedItemChanged += TipoDocumentos_SelectedItemChanged;
            //LISTO //Funcionalidad: PARTE 1) En el combobox debe mostrar los tipos de documentos de la tabla p_tipodoc aplicando un filtro que es id_subserie

            //Dicho valor del id_subserie se toma por la referencia t_lote que haya seleccionado en el datagrid, es decir
            //Primero se selecciona una carpeta (que es el mismo PDF) el cual ya tiene el lote asociado, con ese lote simplemente
            //consultamos el id_suberie y filtramos el combobox.
            //LISTO

            //PARTE 2) Según el tipo de documento que seleccione en el combobox (p_tipodoc) mostrara el formulario que se creara dinamicamente
            //desde el codigo fuente según la tabla p_tipoitem, dicha tabla contiene el nombre del campo, el tipo, el orden, si es requerido o no
            //Existen 4 tipos de campos (Texto, Numerico, Fecha, Selección) cada uno debe tener la validación según corresponda,
            //si es tipo selección debe crear un combobox con las opciones que se encuentran en la tabla p_tiporesp filtrando por el id_item.
            //LISTO

            //PARTE 3) En el momento de guardar se debe validar los campos requeridos y guardar las repuestas en la tabla (t_documento_resp)
            // con el id del documento y el id del item al que corresponda.
            //LISTO

            //PARTE 4) Cuando se guarde el documento con las páginas del PDF seleccionadas, automaticamente deben desaparecer del listview. adicionalmente
            //Si se hizo cambio sobre alguna imágen se debe sobreescribir el archivo PDF con el mismo nombre.
            //LISTO

            //PARTE 5) Cuando se termine de indexar todo el PDF se debe cambiar el estado en la tabla t_carpeta poniendo la letra 'I' y guardando actualizando
            //el campo id_usuario, además debe ingresar un registro en la tabla t_carpeta_estado a manera de log historico para
            //que se pueda conocer la trazabilidad de cada carpeta (PDF)

            KeyDown += IndexLote_KeyDown;
            KeyUp += IndexLote_KeyUp;
            tipoDocumentos.GotFocus += TipoDocumentos_GotFocus;
            IsVisibleChanged += IndexarDocumentos_IsVisibleChanged;
        }

        private void IndexarDocumentos_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        private async void StaticDeepSpeech_OnComando(object sender, ComandoEventArgs e) => await SaveDocument();

        private void TipoDocumentos_GotFocus(object sender, RoutedEventArgs e) => tipoDocumentos.SelectAll();

        private void TipoDocumentos_SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            btnGuardarIndice.IsEnabled = true;
            if (_tiposDoc != null && _tiposDoc.Count != 0 && !string.IsNullOrWhiteSpace(tipoDocumentos.Text) /*cbxArchivadores.SelectedIndex != -1*/)
            {
                if (_adminEdit && _firstAdminSelectionIndex == tipoDocumentos.Text)
                {
                    CleanInputs();
                    foreach (var capturaItem in _selectedDocumentAdmin.Respuesta)
                    {
                        string type = string.Empty;
                        string descripcion = string.Empty;
                        int requerido = 1;
                        ICollection<p_tiporesp> _p_tiporesp = null;
                        //SI EL SISTEMA NO LOGRA TRAER LOS TIPOS DE RESPUESTA, ENTONCES LAS CONSULTA DE LA BASE DE DATOS
                        if (capturaItem.p_tipoitem == null && capturaItem.id_item != null)
                        {
                            using (gdocxEntities context = new gdocxEntities())
                            {
                                var ptipo = context.p_tipoitem.AsNoTracking().Where(p => p.id == capturaItem.id_item).Select(s=> new {s.type,s.descripcion,s.requerido,s.p_tiporesp }).FirstOrDefault();
                                if (ptipo != null)
                                {
                                    type = ptipo.type;
                                    descripcion = ptipo.descripcion;
                                    requerido = ptipo.requerido;
                                    _p_tiporesp = ptipo.p_tiporesp;
                                }
                            }
                        }
                        else
                        {
                            type = capturaItem.p_tipoitem.type;
                            descripcion = capturaItem.p_tipoitem.descripcion;
                            requerido = capturaItem.p_tipoitem.requerido;
                            _p_tiporesp = capturaItem.p_tipoitem.p_tiporesp;
                        }

                        switch (type)
                        {
                            case "NUMERICO":
                                Grid numerico = XamlReader.Parse(XamlWriter.Save(textInput_2)) as Grid;
                                numerico.Visibility = Visibility.Visible;
                                ((Label)numerico.Children[0]).Content = descripcion;
                                string name = $"txt_{descripcion}_numerico_{(requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((TextBox)numerico.Children[1]).Name = name;
                                ((TextBox)numerico.Children[1]).Text = capturaItem.valor.ToUpper().TrimStart(' ').TrimEnd(' ');
                                ((TextBox)numerico.Children[1]).LostFocus -= LostFocus;
                                ((TextBox)numerico.Children[1]).LostFocus += LostFocus;
                                ((TextBox)numerico.Children[1]).GotFocus -= GotFocus;
                                ((TextBox)numerico.Children[1]).GotFocus += GotFocus;
                                if (_viewValues.ContainsKey(name))
                                {
                                    ((TextBox)numerico.Children[1]).Text = _viewValues[name];
                                }
                                dynamicInputsStack.Children.Add(numerico);

                                break;

                            case "SELECCION":
                                Grid seleccion = XamlReader.Parse(XamlWriter.Save(cbxInput)) as Grid;
                                seleccion.Visibility = Visibility.Visible;
                                ((Label)seleccion.Children[0]).Content = descripcion;
                                ObservableCollection<SeleccionItem> cbxItems = new ObservableCollection<SeleccionItem>(_p_tiporesp.OrderBy(x => x.nombre).Select(x => new SeleccionItem { Valor = x.valor, Nombre = x.nombre.ToUpper().Trim(), NombreLimpio = x.nombre.RemoveAccent().ToUpper().Trim() }));
                                ((SfTextBoxExt)seleccion.Children[1]).AutoCompleteSource = cbxItems;
                                ((SfTextBoxExt)seleccion.Children[1]).Name = $"cbx_{descripcion}_seleccion_{(requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((SfTextBoxExt)seleccion.Children[1]).LostFocus -= LostFocus;
                                ((SfTextBoxExt)seleccion.Children[1]).LostFocus += LostFocus;
                                ((SfTextBoxExt)seleccion.Children[1]).GotFocus -= GotFocus;
                                ((SfTextBoxExt)seleccion.Children[1]).GotFocus += GotFocus;
                                //ItemCollection items = ((ComboBox)seleccion.Children[1]).Items;
                                p_tiporesp p_tiporesp = _p_tiporesp.ToList().FirstOrDefault(x =>
                                {
                                    return x.nombre.Trim().ToLower() == capturaItem.valor.Trim().ToLower() || x.valor.Trim().ToLower() == capturaItem.valor.Trim().ToLower();
                                });
                                var selectedValue = p_tiporesp?.nombre;
                                //foreach (p_tiporesp cbi in items)
                                //{
                                //    if (cbi.nombre == capturaItem.valor.TrimStart(' ').TrimEnd(' '))
                                //    {
                                //        ((ComboBox)seleccion.Children[1]).SelectedItem = cbi.valor.TrimStart(' ').TrimEnd(' ');
                                //    }
                                //}
                                ((SfTextBoxExt)seleccion.Children[1]).SelectedValue = selectedValue;
                                dynamicInputsStack.Children.Add(seleccion);
                                break;

                            case "FECHA":
                                Grid fecha = XamlReader.Parse(XamlWriter.Save(dateInput)) as Grid;
                                fecha.Visibility = Visibility.Visible;
                                ((Label)fecha.Children[0]).Content = descripcion;
                                ((SfDatePicker)fecha.Children[1]).Name = $"dtpicker_{descripcion.Replace(".", "")}_fecha_{(requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((SfDatePicker)fecha.Children[1]).Value = DateTime.Parse(capturaItem.valor.ToUpper().TrimStart(' ').TrimEnd(' '));
                                ((SfDatePicker)fecha.Children[1]).LostFocus -= LostFocus;
                                ((SfDatePicker)fecha.Children[1]).LostFocus += LostFocus;
                                ((SfDatePicker)fecha.Children[1]).GotFocus -= GotFocus;
                                ((SfDatePicker)fecha.Children[1]).GotFocus += GotFocus;
                                dynamicInputsStack.Children.Add(fecha);
                                break;

                            case "NOTA":
                                Grid nota = XamlReader.Parse(XamlWriter.Save(textInput)) as Grid;
                                nota.Visibility = Visibility.Visible;

                                ((Label)(nota.Children[0] as StackPanel).Children[0]).Content = descripcion;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).Name = $"txt_{descripcion}_nota_{(requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).Text = capturaItem.valor.ToUpper().TrimStart(' ').TrimEnd(' ');
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).LostFocus -= LostFocus;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).LostFocus += LostFocus;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).GotFocus -= GotFocus;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).GotFocus += GotFocus;

                                dynamicInputsStack.Children.Add(nota);
                                break;

                            case "CHECK":

                                Grid check = XamlReader.Parse(XamlWriter.Save(chkInput)) as Grid;
                                check.Visibility = Visibility.Visible;
                                (check.Children[0] as CheckBox).Name = $"chk_{descripcion}_chk_{(requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                (check.Children[0] as CheckBox).Content = descripcion;
                                (check.Children[0] as CheckBox).IsChecked = Convert.ToBoolean(capturaItem.valor);
                                dynamicInputsStack.Children.Add(check);
                                break;

                            default:
                                break;
                        }
                    }
                    Visibility = Visibility.Visible;
                }
                else
                {
                    // _selectedTipoDoc = _tiposDoc[cbxArchivadores.SelectedIndex];
                    _selectedTipoDoc = _tiposDoc[_tiposDoc.ToList().FindIndex(x =>
                    {
                        string selectedValue = (d as SfTextBoxExt).SelectedValue as string;
                        return x.nombre.Trim().ToLower().RemoveAccent() == selectedValue.ToString().Trim().ToLower().RemoveAccent();
                    })];
                    _tipoItem = _selectedTipoDoc.p_tipoitem.Where(x => x.activo != 0).OrderBy(x => x.orden).ToList();

                    CleanInputs();
                    foreach (var capturaItem in _tipoItem)
                    {
                        switch (capturaItem.type)
                        {
                            case "NUMERICO":
                                Grid numerico = XamlReader.Parse(XamlWriter.Save(textInput_2)) as Grid;
                                numerico.Visibility = Visibility.Visible;
                                ((Label)numerico.Children[0]).Content = capturaItem.descripcion;
                                string name = $"txt_{capturaItem.descripcion}_numerico_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((TextBox)numerico.Children[1]).Name = name;
                                ((TextBox)numerico.Children[1]).LostFocus -= LostFocus;
                                ((TextBox)numerico.Children[1]).LostFocus += LostFocus;
                                ((TextBox)numerico.Children[1]).GotFocus -= GotFocus;
                                ((TextBox)numerico.Children[1]).GotFocus += GotFocus;
                                if (_viewValues.ContainsKey(name))
                                {
                                    ((TextBox)numerico.Children[1]).Text = _viewValues[name];
                                }
                                dynamicInputsStack.Children.Add(numerico);
                                break;

                            case "SELECCION":
                                Grid seleccion = XamlReader.Parse(XamlWriter.Save(cbxInput)) as Grid;
                                seleccion.Visibility = Visibility.Visible;
                                ((Label)seleccion.Children[0]).Content = capturaItem.descripcion;
                                ((SfTextBoxExt)seleccion.Children[1]).LostFocus -= LostFocus;
                                ((SfTextBoxExt)seleccion.Children[1]).LostFocus += LostFocus;
                                ((SfTextBoxExt)seleccion.Children[1]).GotFocus -= GotFocus;
                                ((SfTextBoxExt)seleccion.Children[1]).GotFocus += GotFocus;
                                if (capturaItem.p_tiporesp.Count != 0)
                                {
                                    ObservableCollection<SeleccionItem> cbxItems = new ObservableCollection<SeleccionItem>(capturaItem.p_tiporesp.OrderBy(x => x.nombre).Select(x => new SeleccionItem { Valor = x.valor, Nombre = x.nombre.ToUpper().Trim(), NombreLimpio = x.nombre.RemoveAccent().ToUpper().Trim() }));
                                    ((SfTextBoxExt)seleccion.Children[1]).AutoCompleteSource = cbxItems;
                                }
                                ((SfTextBoxExt)seleccion.Children[1]).Name = $"cbx_{capturaItem.descripcion}_seleccion_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");

                                //int selectedIndex = capturaItem.p_tiporesp.ToList().FindIndex(x => x.nombre == capturaItem.descripcion.TrimStart(' ').TrimEnd(' '));
                                //foreach (p_tiporesp cbi in items)
                                //{
                                //    if (cbi.nombre == capturaItem.valor.TrimStart(' ').TrimEnd(' '))
                                //    {
                                //        ((ComboBox)seleccion.Children[1]).SelectedItem = cbi.valor.TrimStart(' ').TrimEnd(' ');
                                //    }
                                //}
                                p_tiporesp p_tiporesp = capturaItem.p_tiporesp.FirstOrDefault(x => x.nombre.ToLower() == capturaItem.descripcion.TrimStart(' ').TrimEnd(' '));

                                if (p_tiporesp != null)
                                {
                                    ((SfTextBoxExt)seleccion.Children[1]).SelectedValue = p_tiporesp.nombre;
                                }

                                dynamicInputsStack.Children.Add(seleccion);
                                break;

                            case "FECHA":
                                Grid fecha = XamlReader.Parse(XamlWriter.Save(dateInput)) as Grid;
                                fecha.Visibility = Visibility.Visible;
                                ((Label)fecha.Children[0]).Content = capturaItem.descripcion;
                                string dateName = $"dtpicker_{capturaItem.descripcion.Replace(".", "")}_fecha_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((SfDatePicker)fecha.Children[1]).Name = dateName;
                                ((SfDatePicker)fecha.Children[1]).LostFocus -= LostFocus;
                                ((SfDatePicker)fecha.Children[1]).LostFocus += LostFocus;
                                ((SfDatePicker)fecha.Children[1]).GotFocus -= GotFocus;
                                ((SfDatePicker)fecha.Children[1]).GotFocus += GotFocus;
                                dynamicInputsStack.Children.Add(fecha);
                                if (_viewValues.ContainsKey(dateName))
                                {
                                    ((SfDatePicker)fecha.Children[1]).Value = DateTime.Parse(_viewValues[dateName]);
                                }
                                break;

                            case "NOTA":
                                Grid nota = XamlReader.Parse(XamlWriter.Save(textInput)) as Grid;
                                nota.Visibility = Visibility.Visible;

                                ((Label)(nota.Children[0] as StackPanel).Children[0]).Content = capturaItem.descripcion;
                                string nameNota = $"txt_{capturaItem.descripcion}_nota_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).Name = nameNota;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).LostFocus -= LostFocus;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).LostFocus += LostFocus;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).GotFocus -= GotFocus;
                                ((TextBox)(nota.Children[0] as StackPanel).Children[1]).GotFocus += GotFocus;
                                dynamicInputsStack.Children.Add(nota);
                                if (_viewValues.ContainsKey(nameNota))
                                {
                                    ((TextBox)(nota.Children[0] as StackPanel).Children[1]).Text = _viewValues[nameNota];
                                }
                                break;

                            case "CHECK":
                                Grid check = XamlReader.Parse(XamlWriter.Save(chkInput)) as Grid;
                                check.Visibility = Visibility.Visible;
                                (check.Children[0] as CheckBox).Name = $"chk_{capturaItem.descripcion}_chk_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                                (check.Children[0] as CheckBox).Content = capturaItem.descripcion;
                                dynamicInputsStack.Children.Add(check);
                                break;

                            default:
                                break;
                        }
                    }
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
                    if (((StackPanel)parent).Children.Count >= 2)
                    {
                        UIElement uIElement = ((StackPanel)parent).Children[1];
                        if (uIElement is ListBox)
                        {
                            var suggestionsList = (ListBox)uIElement;
                            if (suggestionsList.Visibility == Visibility.Visible)
                            {
                                suggestionsList.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
            StaticDeepSpeech.StopRecording(true);
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            var _selectedTextBox = sender is TextBox box ? box : null;
            if (_selectedTextBox != null && _selectedTextBox.Name.Contains("_nota_"))
            {
                object parent = _selectedTextBox.Parent;
                if (parent != null && typeof(StackPanel) == parent.GetType())
                {
                    var suggestionsList = (ListBox)((StackPanel)parent).Children[2];
                    StaticDeepSpeech.SetFocus(sender, Dispatcher, suggestionsList);
                }
                else
                {
                    StaticDeepSpeech.SetFocus(sender, Dispatcher); //dispatcher usado para evitar problemas con los threads de DeepSpeech.
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
        }

        /// <summary>
        /// Selección del tipo de dcumento que carga los campos de forma dinámica.
        /// </summary>
        /// <param name="selectedItem"></param>
        internal void SetItem(ControlCalidadIListItem selectedItem)
        {
            //carga los tipos documentos disponibles

            _adminEdit = true;
            tipoDocumentos.Text = "";
            CleanInputs();
            chkOmitirSelección.Visibility = Visibility.Hidden;
            chkOmitirSelección.IsChecked = false;
            //cbxArchivadores.SelectionChanged -= CbxArchivadores_SelectionChanged;
            //if (_isAdmin)
            //{
            //    _tipoItem = new List<p_tipoitem>();
            //    foreach (var item in selectedItem.Respuesta)
            //    {
            //        _tipoItem.Add(item.p_tipoitem);
            //    }
            //}
            _selectedDocumentAdmin = selectedItem;

            _firstAdminSelectionIndex = _tiposDoc.ToList().Find(x => x.nombre == selectedItem.Archivador)?.nombre; //para saber si el admin cambió de tipo de documento
                                                                                                                   //cbxArchivadores.SelectedIndex = _firstAdminSelectionIndex;
            if (_firstAdminSelectionIndex != null) tipoDocumentos.SelectedValue = _firstAdminSelectionIndex;
            else MessageBox.Show("La serie documental no tiene tipologías, por favor contacte al administrador", "AlphaAI", MessageBoxButton.OK, MessageBoxImage.Warning);

            //cbxArchivadores.SelectionChanged += CbxArchivadores_SelectionChanged;
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

        /// <summary>
        /// Carga los tipos de documento al combobox.
        /// </summary>
        public void LoadData()
        {
            int? idSubSerie = GlobalClass.Carpeta.IdSubSerie;
            _tiposDoc = new ObservableCollection<p_tipodoc>(EntitiesRepository.Entities.p_tipodoc.AsNoTracking().Include("p_tipoitem").Include("p_tipoitem.p_tiporesp").Where(x => x.id_subserie == idSubSerie).ToList());
            IEnumerable<string> tipos = _tiposDoc.OrderBy(x => x.nombre).Select(x => x.nombre);

            Dispatcher.Invoke(() =>
            {
                tipoDocumentos.AutoCompleteSource = tipos;
                tipoDocumentos.Text = "";
            });
        }

        private void IndexarDocumentos_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Reinicia el view si se cancela el indexado del documento seleccionado.
        /// </summary>
        public void Reset()
        {
            try
            {
                foreach (UIElement element in dynamicInputsStack.Children)
                {
                    element.GotFocus -= GotFocus;
                }
                dynamicInputsStack.Children.Clear();
                //cbxArchivadores.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Limpia los campos del view dinámico-
        /// </summary>
        public void CleanInputs(bool resetIndexView = false)
        {
            if (!resetIndexView)
            {
                if (_indexadorView != null)
                {
                    _indexadorView.ResetView();
                }
            }

            UIElementCollection childrens = dynamicInputsStack.Children;
            System.Collections.IList list = childrens;
            _viewValues.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (((UIElement)list[i]).Visibility == Visibility.Visible)
                {
                    if (((Grid)list[i]).Children[0] is StackPanel panel)
                    {
                        if (panel.Children[1] is TextBox textbox)
                        {
                            _viewValues.Add(textbox.Name, textbox.Text);
                        }
                        else if (panel.Children[1] is SfDatePicker date)
                        {
                            if (date.Value != null)
                            {
                                _viewValues.Add(date.Name, date.Value.ToString());
                            }
                        }
                        dynamicInputsStack.Children.Remove(((UIElement)list[i]));
                        i = -1;
                    }
                    else
                    {
                        if (((Grid)list[i]).Children.Count == 2)
                        {
                            if (((Grid)list[i]).Children[1] is SfDatePicker date)
                            {
                                if (date.Value != null)
                                {
                                    _viewValues.Add(date.Name, date.Value.ToString());
                                }
                            }
                            else if (((Grid)list[i]).Children[1] is TextBox textbox)
                            {
                                _viewValues.Add(textbox.Name, textbox.Text);
                            }
                        }
                        dynamicInputsStack.Children.Remove(((UIElement)list[i]));
                        i = -1;
                    }
                }
            }
        }

        private async void BtnGuardarIndice_Click(object sender, RoutedEventArgs e)
        {
            await SaveDocument();
        }

        private async Task SaveDocument()
        {
            btnGuardarIndice.IsEnabled = false;
            try
            {
                if (_adminEdit)
                {
                    if (IsValidForm())
                    {
                        UpdateDocumentAdminAsync(_selectedDocumentAdmin.Documento);
                        tipoDocumentos.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show(_invalidFormMesssage, "Formulario incompleto.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    if (_indexadorView != null)
                    {
                        int numSeleccionados = _indexadorView.lbxPdfImages.SelectedItems.Count;
                        if (numSeleccionados == 0 && !chkOmitirSelección.IsChecked.Value)
                        {
                            MessageBox.Show("Debe seleccionar en la galería las imágenes del documento.");
                        }
                        else if (!_indexadorView.validaRango() && !chkOmitirSelección.IsChecked.Value)
                        {
                            MessageBox.Show("Rango no Valido, debe seleccinar imágenes consecutivas, sin saltos númericos.");
                        }
                        else
                        {
                            if (IsValidForm()) //verifica el formulario sea correcto
                            {
                                if (_selectedTipoDoc.multiterceros == 1)
                                {
                                    MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea adicionar la información complementaria?", "Importante", MessageBoxButton.YesNo);
                                    if (messageBoxResult == MessageBoxResult.Yes)
                                    {
                                        _documento = SaveDocumento();
                                        _informacionComplementaria = new InformacionComplementaria();
                                        _informacionComplementaria.SetDocumento(_documento, GlobalClass.selPagInicial, GlobalClass.selPagFinal);

                                        _informacionComplementaria.Closed += _informacionComplementaria_Closed;
                                        _informacionComplementaria.Show();
                                    }
                                    else
                                    {
                                        t_documento documento = SaveDocumento();
                                        if (documento != null)
                                        {
                                            await SaveIndexChangesAsync(documento);
                                            tipoDocumentos.Text = string.Empty;
                                        }
                                    }
                                }
                                else
                                {
                                    t_documento documento = SaveDocumento();
                                    if(documento != null)
                                    {
                                        await SaveIndexChangesAsync(documento);
                                        tipoDocumentos.Text = string.Empty;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(_invalidFormMesssage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }

            btnGuardarIndice.IsEnabled = true;
            chkOmitirSelección.IsChecked = false;
        }

        private async void _informacionComplementaria_Closed(object sender, EventArgs e)
        {
            await SaveIndexChangesAsync(_documento);
        }

        /// <summary>
        /// Verifica que los campos del formulario sean correctos.
        /// </summary>
        /// <returns>True si el formulario cumple los requerimientos del documento..</returns>
        public bool IsValidForm()
        {
            if (tipoDocumentos.SelectedValue == null)
            {
                _invalidFormMesssage = $"Debe seleccionar un tipo documental";
                tipoDocumentos.Focus();
                return false;
            }
            if (_adminEdit && _firstAdminSelectionIndex == tipoDocumentos.SelectedValue.ToString())
            {
                _tipoItem = new List<p_tipoitem>();
                foreach (var item in _selectedDocumentAdmin.Respuesta)
                {
                    p_tipoitem _TipoItem = new p_tipoitem();
                    //SI EL SISTEMA NO LOGRA TRAER LOS TIPOS DE RESPUESTA, ENTONCES LAS CONSULTA DE LA BASE DE DATOS
                    if (item.p_tipoitem == null && item.id_item != null)
                    {
                        using (gdocxEntities context = new gdocxEntities())
                        {
                            p_tipoitem ptipo = context.p_tipoitem.AsNoTracking().Where(p => p.id == item.id_item).FirstOrDefault();
                            if (ptipo != null)
                            {
                                _TipoItem = ptipo;
                            }
                        }
                    }
                    else
                    {
                        _TipoItem = item.p_tipoitem;
                    }

                    _tipoItem.Add(_TipoItem);
                }
            }
            if (_tipoItem == null)
            {
                _invalidFormMesssage = $"Debe seleccionar un tipo documental";
                tipoDocumentos.Focus();
                return false;
            }

            foreach (var capturaItem in _tipoItem)
            {
                switch (capturaItem.type)
                {
                    case "NUMERICO":
                        string name = $"txt_{capturaItem.descripcion}_numerico_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                        var txtNumerico = dynamicInputsStack.FindControl<TextBox>(name);
                        if (capturaItem.requerido == 1 && string.IsNullOrWhiteSpace(txtNumerico.Text.ToUpper().TrimStart(' ').TrimEnd(' ')))
                        {
                            _invalidFormMesssage = $"El campo {capturaItem.descripcion} no puede estar vacío.";
                            return false;
                        }
                        //VALIDA QUE LO QUE HAYA DIGITADO SEA NÚMERICO Y NO LETRAS
                        int numeroDigitado = GlobalClass.GetNumber(txtNumerico.ToString(), -1);
                        if (numeroDigitado == -1 && !string.IsNullOrWhiteSpace(txtNumerico.Text.ToUpper().TrimStart(' ').TrimEnd(' ')))
                        {
                            _invalidFormMesssage = $"El campo {capturaItem.descripcion} debe ser númerico.";
                            return false;
                        }
                        break;

                    case "SELECCION":
                        var cbxSeleccion = dynamicInputsStack.FindControl<SfTextBoxExt>($"cbx_{capturaItem.descripcion}_seleccion_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));
                        if ((cbxSeleccion.SelectedValue?.ToString().RemoveAccent() != cbxSeleccion.Text.RemoveAccent() && capturaItem.requerido == 1) ||(cbxSeleccion.SelectedValue == null && capturaItem.requerido == 1) || (string.IsNullOrWhiteSpace(cbxSeleccion.SelectedValue?.ToString()) && capturaItem.requerido == 1))
                        {
                            _invalidFormMesssage = $"El campo {capturaItem.descripcion} no puede estar sin selección.";
                            return false;
                        }
                        else if (cbxSeleccion.SelectedValue!=null && (cbxSeleccion.SelectedValue?.ToString().RemoveAccent() != cbxSeleccion.Text.RemoveAccent()))
                        {
                            _invalidFormMesssage = $"El texto del campo {capturaItem.descripcion} no coincide con la selección.";
                            return false;
                        }
                        break;

                    case "FECHA":
                        string name1 = $"dtpicker_{capturaItem.descripcion.Replace(".", "")}_fecha_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                        var dtPicker = dynamicInputsStack.FindControl<SfDatePicker>(name1);
                        if (capturaItem.requerido == 1 && string.IsNullOrWhiteSpace(dtPicker.Value.ToString().ToUpper().TrimStart(' ').TrimEnd(' ')))
                        {
                            _invalidFormMesssage = $"El campo {capturaItem.descripcion} no puede estar vacío.";
                            return false;
                        }
                        break;

                    case "NOTA":
                        var myTextBlock = dynamicInputsStack.FindControl<TextBox>($"txt_{capturaItem.descripcion}_nota_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));
                        if (capturaItem.requerido == 1 && string.IsNullOrWhiteSpace(myTextBlock.Text.ToUpper().TrimStart(' ').TrimEnd(' ')))
                        {
                            _invalidFormMesssage = $"El campo {capturaItem.descripcion} no puede estar vacío.";
                            return false;
                        }
                        if (myTextBlock.Text.Length > 255)
                        {
                            _invalidFormMesssage = $"El campo {capturaItem.descripcion} no puede superar los 255 caracteres..";
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        private DocumentoGeneral getDocumentoGeneral(DocumentoGeneral d, string tipo, string desc, string val)
        {
            desc = desc.Trim().ToUpper();
            if (tipo == "NUMERICO" && (desc == "FOLIO INICIAL" || desc == "FOLIO INICIA" || desc == "FOLIO INICIO" || desc == "FOLIO INI"))
            {
                d.folio_ini = GlobalClass.GetNumber(val);
            }
            else if (tipo == "NUMERICO" && (desc == "FOLIO FINAL" || desc == "FOLIO FINALIZA" || desc == "FOLIO TERMINA" || desc == "FOLIO FIN"))
            {
                d.folio_fin = GlobalClass.GetNumber(val);
            }
            else if (tipo == "NUMERICO" && (desc == "ITEM" || desc == "ÍTEM"))
            {
                d.item = GlobalClass.GetNumber(val);
            }
            else if (tipo == "FECHA" && (desc == "FECHA" || desc == "FECHA DEL DOCUMENTO" || desc == "F. DOCUMENTO"))
            {
                d.fecha = DateTime.Parse(val);
            }
            else if (tipo == "NOTA" && (desc == "OBSERVACIONES" || desc == "OBSERVACION" || desc == "OBSERVACIÓN"))
            {
                d.observacion = val;
            }
            else if (tipo == "NOTA" && (desc.Contains("NRO DOCUMENTO") || desc.Contains("NUMERO DOCUMENTO") || desc.Contains("NÚMERO DOCUMENTO") || desc.Contains("No. DOCUMENTO")))
            {
                d.nro_doc = val;
            }
            else if (tipo == "NOTA" && (desc.Contains("NOM DOCUMENTO") || desc.Contains("NOMBRE DOCUMENTO")))
            {
                d.nom_doc = val;
            }
            return d;
        }

        private void UpdateDocumentAdminAsync(t_documento documento)
        {
            //si cambia el tipo documental se actualiza en la base de datos
            if (_firstAdminSelectionIndex != tipoDocumentos.SelectedValue.ToString())
            {
                documento.id_tipodoc = _selectedTipoDoc.id;
                EntitiesRepository.Entities.t_documento.AddOrUpdateExtension(documento);
            }
            if (_adminEdit && _firstAdminSelectionIndex == tipoDocumentos.SelectedValue.ToString())
            {
                _tipoItem = new List<p_tipoitem>();
                foreach (var item in _selectedDocumentAdmin.Respuesta)
                {
                    _tipoItem.Add(item.p_tipoitem);
                }
            }
            if (_adminEdit /*&& _firstAdminSelectionIndex == cbxArchivadores.SelectedIndex*/)
            {
                foreach (var respuesta in documento.t_documento_resp.ToList())
                {
                    var resp = EntitiesRepository.Entities.t_documento_resp.Find(respuesta.id);
                    if (resp != null)
                    {
                        EntitiesRepository.Entities.t_documento_resp.Remove(resp);
                    }
                    else
                    {
                        Telemetry.TrackException(new Exception($"Resp null: {documento.id}"));
                    }
                }
            }
            EntitiesRepository.Entities.SaveChanges();

            DocumentoGeneral docGeneral = new DocumentoGeneral();
            docGeneral.fecha_regdoc = DateTime.Now;
            foreach (var capturaItem in _tipoItem)
            {
                Console.WriteLine(capturaItem.descripcion);
                switch (capturaItem?.type)
                {
                    case "NUMERICO":
                        string name = $"txt_{capturaItem.descripcion}_numerico_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                        var txtNumerico = dynamicInputsStack.FindControl<TextBox>(name);
                        //Convierte el valor a Numero
                        int numeroDigitado = GlobalClass.GetNumber(txtNumerico.Text.TrimStart(' ').TrimEnd(' ').ToString(), 0);
                        EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                        {
                            id_item = capturaItem.id,
                            valor = numeroDigitado.ToString(),
                            id_documento = documento.id
                        });

                        docGeneral = getDocumentoGeneral(docGeneral, "NUMERICO", capturaItem.descripcion, numeroDigitado.ToString()); //Verifica si es dato general

                        break;

                    case "SELECCION":
                        var cbxSeleccion = dynamicInputsStack.FindControl<SfTextBoxExt>($"cbx_{capturaItem.descripcion}_seleccion_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));

                        EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                        {
                            id_item = capturaItem.id,
                            valor = !string.IsNullOrWhiteSpace(cbxSeleccion.SelectedValue?.ToString()) ? cbxSeleccion.SelectedValue?.ToString().TrimStart(' ').TrimEnd(' ') : string.Empty,
                            id_documento = documento.id
                        });

                        break;

                    case "FECHA":
                        string name1 = $"dtpicker_{capturaItem.descripcion.Replace(".", "")}_fecha_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                        var dtPicker = dynamicInputsStack.FindControl<SfDatePicker>(name1);
                        var fechaOk = Convert.ToDateTime(dtPicker.Value.ToString().TrimStart(' ').TrimEnd(' ')).ToString("yyyy-MM-dd");

                        EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                        {
                            id_item = capturaItem.id,
                            valor = fechaOk,
                            id_documento = documento.id
                        });

                        docGeneral = getDocumentoGeneral(docGeneral, "FECHA", capturaItem.descripcion, fechaOk); //Verifica si es dato general

                        break;

                    case "NOTA":
                        var myTextBlock = dynamicInputsStack.FindControl<TextBox>($"txt_{capturaItem.descripcion}_nota_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));
                        string val = myTextBlock.Text.ToUpper().TrimStart(' ').TrimEnd(' ');
                        EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                        {
                            id_item = capturaItem.id,
                            valor = val,
                            id_documento = documento.id
                        });

                        docGeneral = getDocumentoGeneral(docGeneral, "NOTA", capturaItem.descripcion, val); //Verifica si es dato general

                        break;

                    case "CHECK":
                        var dynamicCheck = dynamicInputsStack.FindControl<CheckBox>($"chk_{capturaItem.descripcion}_chk_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));

                        EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                        {
                            id_item = capturaItem.id,
                            valor = dynamicCheck.IsChecked.ToString(),
                            id_documento = documento.id
                        });
                        break;
                }
            }
            if (!_adminEdit)
            {
                CleanInputs();
                //cbxArchivadores.SelectedIndex = 1;
                //cbxArchivadores.SelectedIndex = 0;
                _indexadorView.OcultaImgIndexadas();
                _indexadorView.RemoveFromPdf();
            }
            else
            {
                EntitiesRepository.Entities.SaveChanges();
                Visibility = Visibility.Hidden;
                _controlCalidadView.UpdateWithReset();
            }
            Task.Run(() =>
            {
                using (gdocxEntities context = new gdocxEntities())
                {
                    var currentDocument = context.t_documento.FirstOrDefault(x => x.id == documento.id);
                    currentDocument.item = docGeneral.item;
                    currentDocument.folio_ini = docGeneral.folio_ini;
                    currentDocument.folio_fin = docGeneral.folio_fin;
                    currentDocument.fecha = docGeneral.fecha;
                    currentDocument.nro_doc = docGeneral.nro_doc;
                    currentDocument.nom_doc = docGeneral.nom_doc;
                    currentDocument.observacion = docGeneral.observacion;
                    currentDocument.fecha_regdoc = DateTime.Now;

                    context.SaveChanges();
                }

            });
        }

        private async Task SaveIndexChangesAsync(t_documento documento)
        {
            try
            {
                DocumentoGeneral docGeneral = new DocumentoGeneral();
                docGeneral.fecha_regdoc = DateTime.Now;
                foreach (var capturaItem in _tipoItem)
                {
                    Console.WriteLine(capturaItem.descripcion);
                    switch (capturaItem.type)
                    {
                        case "NUMERICO":
                            string name = $"txt_{capturaItem.descripcion}_numerico_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                            var txtNumerico = dynamicInputsStack.FindControl<TextBox>(name);

                            if (capturaItem.requerido == 1 && string.IsNullOrWhiteSpace(txtNumerico.Text.ToUpper().TrimStart(' ').TrimEnd(' ')))
                            {
                                throw new ArgumentException($"El campo {capturaItem.descripcion} no puede estar vacío.");
                            }
                            else
                            {
                                //Convierte el valor a Numero
                                int numeroDigitado = GlobalClass.GetNumber(txtNumerico.Text.TrimStart(' ').TrimEnd(' ').ToString(), 0);
                                EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                                {
                                    id_item = capturaItem.id,
                                    valor = numeroDigitado.ToString(),
                                    id_documento = documento.id
                                });
                                docGeneral = getDocumentoGeneral(docGeneral, "NUMERICO", capturaItem.descripcion, numeroDigitado.ToString()); //Verifica si es dato general
                            }
                            break;

                        case "SELECCION":
                            var cbxSeleccion = dynamicInputsStack.FindControl<SfTextBoxExt>($"cbx_{capturaItem.descripcion}_seleccion_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));
                            if ((cbxSeleccion.SelectedValue?.ToString().RemoveAccent() != cbxSeleccion.Text.RemoveAccent() && capturaItem.requerido == 1) || (cbxSeleccion.SelectedValue == null && capturaItem.requerido == 1) || (string.IsNullOrWhiteSpace(cbxSeleccion.SelectedValue?.ToString()) && capturaItem.requerido == 1))
                            {
                                throw new ArgumentException($"El campo {capturaItem.descripcion} no puede estar sin selección.");
                            }
                            else
                            {
                                var selectedValued = cbxSeleccion.SelectedValue != null && !string.IsNullOrWhiteSpace(cbxSeleccion.SelectedValue.ToString()) ? cbxSeleccion.SelectedValue.ToString() : string.Empty;
                                EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                                {
                                    id_item = capturaItem.id,
                                    valor = selectedValued.ToString(),
                                    id_documento = documento.id
                                });
                            }
                            break;

                        case "FECHA":
                            string name1 = $"dtpicker_{capturaItem.descripcion.Replace(".", "")}_fecha_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", "");
                            var dtPicker = dynamicInputsStack.FindControl<SfDatePicker>(name1);
                            if (capturaItem.requerido == 1 && string.IsNullOrWhiteSpace(dtPicker.Value.ToString().ToUpper().TrimStart(' ').TrimEnd(' ')))
                            {
                                throw new ArgumentException($"El campo {capturaItem.descripcion} no puede estar vacío.");
                            }
                            else
                            {
                                var fechaOk = Convert.ToDateTime(dtPicker.Value.ToString().TrimStart(' ').TrimEnd(' ')).ToString("yyyy-MM-dd");
                                EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                                {
                                    id_item = capturaItem.id,
                                    valor = fechaOk,
                                    id_documento = documento.id
                                });
                                docGeneral = getDocumentoGeneral(docGeneral, "FECHA", capturaItem.descripcion, fechaOk); //Verifica si es dato general
                            }
                            break;

                        case "NOTA":
                            var myTextBlock = dynamicInputsStack.FindControl<TextBox>($"txt_{capturaItem.descripcion}_nota_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));
                            if (capturaItem.requerido == 1 && string.IsNullOrWhiteSpace(myTextBlock.Text.ToUpper().TrimStart(' ').TrimEnd(' ')))
                            {
                                throw new ArgumentException($"El campo {capturaItem.descripcion} no puede estar vacío.");
                            }
                            else
                            {
                                string val = myTextBlock.Text.ToUpper().TrimStart(' ').TrimEnd(' ');
                                EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                                {
                                    id_item = capturaItem.id,
                                    valor = val,
                                    id_documento = documento.id
                                });

                                docGeneral = getDocumentoGeneral(docGeneral, "NOTA", capturaItem.descripcion, val); //Verifica si es dato general
                            }
                            break;

                        case "CHECK":
                            var dynamicCheck = dynamicInputsStack.FindControl<CheckBox>($"chk_{capturaItem.descripcion}_chk_{(capturaItem.requerido == 1 ? "requerido" : "")}_".Replace(" ", ""));

                            EntitiesRepository.Entities.t_documento_resp.Add(new t_documento_resp
                            {
                                id_item = capturaItem.id,
                                valor = dynamicCheck.IsChecked.ToString(),
                                id_documento = documento.id
                            });
                            break;
                    }
                }
                await EntitiesRepository.Entities.SaveChangesAsync();
                if (!chkOmitirSelección.IsChecked.Value)
                {
                    if (_informacionComplementaria != null)
                    {
                        _indexadorView.BloqueaRango(_informacionComplementaria.SelPagInicial, _informacionComplementaria.SelPagFinal);
                    }
                    else
                    {
                        _indexadorView.BloqueaRango(GlobalClass.selPagInicial, GlobalClass.selPagFinal);   //Adiciona el rango a las paginas ya indexadas sin conultar la BD
                    }
                }
                CleanInputs();
                _contadorIndexados++;
                if (_indexadoView)
                {
                    Dispatcher.Invoke(() => lblContadorIndexados.Content = $"Total indexados: {_contadorIndexados}");
                }
                //cbxArchivadores.SelectedIndex = 1;
                //cbxArchivadores.SelectedIndex = 0;
                _indexadorView.OcultaImgIndexadas();
                _indexadorView.RemoveFromPdf();
                tipoDocumentos.Text = string.Empty;
                _informacionComplementaria = null;
                Task.Run(() =>
                {
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        var currentDocument = context.t_documento.FirstOrDefault(x => x.id == documento.id);
                        currentDocument.item = docGeneral.item;
                        currentDocument.folio_ini = docGeneral.folio_ini;
                        currentDocument.folio_fin = docGeneral.folio_fin;
                        currentDocument.fecha = docGeneral.fecha;
                        currentDocument.nro_doc = docGeneral.nro_doc;
                        currentDocument.nom_doc = docGeneral.nom_doc;
                        currentDocument.observacion = docGeneral.observacion;
                        currentDocument.fecha_regdoc = DateTime.Now;

                        context.SaveChanges();
                    }
                });
            }
            catch (FormatException ex)
            {
                /*EntitiesRepository.Entities.Dispose();
                EntitiesRepository.Context = new gdocxEntities();*/ //reinicio de contexto para descartar cambios si falla el formato
                Telemetry.TrackException(ex);
                MessageBox.Show("Formato de la fecha incorrecto. Formato correcto : dd/mm/yyyy");
            }
            catch (Exception ex)
            {
                /*EntitiesRepository.Entities.Dispose();
                EntitiesRepository.Context = new gdocxEntities();*/ //reinicio de contexto para descartar cambios si falla el formato
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Guarda los cambios del documento.
        /// </summary>
        /// <returns></returns>
        private t_documento SaveDocumento()
        {
            if(GlobalClass.Carpeta.IdCarptera == 0)
            {
                MessageBox.Show("No se encontró el ID de carpeta");
                return null;
            }

            t_documento documento = new t_documento
            {
                id_tipodoc = _selectedTipoDoc.id,
                id_carpeta = GlobalClass.Carpeta.IdCarptera,
                pag_ini = GlobalClass.selPagInicial,
                pag_fin = GlobalClass.selPagFinal,
                fecha_regdoc = DateTime.Now,
                requiere_seleccion = true
            };
            if (chkOmitirSelección.IsChecked.Value)
            {
                documento.pag_ini = 0;
                documento.pag_fin = 0;
                documento.requiere_seleccion = false;
            }

            EntitiesRepository.Entities.t_documento.Add(documento);
            //await EntitiesRepository.Entities.SaveChangesAsync();
            //if (!chkOmitirSelección.IsChecked.Value)
            //{
            //    _indexadorView.BloqueaRango(GlobalClass.selPagInicial, GlobalClass.selPagFinal);   //Adiciona el rango a las paginas ya indexadas sin conultar la BD
            //}
            return documento;
        }

        internal void SetIndexados(int count, bool indexadoView)
        {
            _indexadoView = indexadoView;
            if (_indexadoView)
            {
                _contadorIndexados = count;
                Dispatcher.Invoke(() => lblContadorIndexados.Content = $"Total indexados: {_contadorIndexados}");
            }
        }

        /// <summary>
        /// Indica el parent view para acutalizar en guardados.
        /// </summary>
        /// <param name="menuPrincipal"></param>
        internal void SetIndexadorView(IndexadorView menuPrincipal)
        {
            _indexadorView = menuPrincipal;
        }

        /// <summary>
        /// Indica el parent de tipo control calidad.
        /// </summary>
        /// <param name="controlCalidadView"></param>
        internal void SetControlCalidad(ControlCalidadRevisionView controlCalidadView)
        {
            _controlCalidadView = controlCalidadView;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void chkOmitirSelección_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void tipoDocumentos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
            }
        }
    }

    public static class UIElementExtensions
    {
        public static T FindControl<T>(this UIElement parent, string ControlName, bool useContains = false) where T : FrameworkElement
        {
            if (parent == null)
                return null;
            if (useContains)
            {
                if (parent.GetType() == typeof(T) && ((T)parent).Name.Contains(ControlName))
                {
                    return (T)parent;
                }
            }
            else
            {
                if (parent.GetType() == typeof(T) && ((T)parent).Name == ControlName)
                {
                    return (T)parent;
                }
            }
            T result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                try
                {
                    if (FindControl<T>(child, ControlName) != null)
                    {
                        result = FindControl<T>(child, ControlName);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //Telemetry.TrackException(ex);
                }
            }
            return result;
        }
    }
}