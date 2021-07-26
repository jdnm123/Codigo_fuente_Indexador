using Gestion.DAL;
using Indexai.Models;
using Indexai.Services;
using NumbersHelper;
using Syncfusion.Data.Extensions;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.PdfViewer;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Indexai.Views
{
    /// <summary>
    /// Interaction logic for BusquedaVozView.xaml
    /// </summary>
    public partial class BusquedaVozView : UserControl
    {
        private List<string> _tiposDoc;
        private bool _alreadyDown = false;
        private List<BusquedaModel> _documentos;
        private BusquedaModel _selectedDocument;
        private PdfViewerControl _pdfViewer;
        private PdfLoadedDocument _loadedDocument;
        private CancellationTokenSource tokenSource;
        private CancellationToken _loadImagesToken;
        private readonly ObservableCollection<PDFPages> _pages = new ObservableCollection<PDFPages>(new List<PDFPages>());
        private string _pdfName;

        public BusquedaVozView()
        {
            InitializeComponent();
            Loaded += BusquedaVozView_Loaded;
            consultaExpander.Expanded += ConsultaExpander_Expanded;
            expanderResultados.Expanded += ExpanderResultados_Expanded;
            gridBusqueda.SelectionChanged += GridBusqueda_SelectionChanged;
            KeyDown += IndexLote_KeyDown;
            KeyUp += IndexLote_KeyUp;
            lbxPdfImages.ItemsSource = _pages;
            lbxPdfImages.SelectionChanged += LbxPdfImages_SelectionChanged;
            pdfviewer.ToolbarSettings.ShowAnnotationTools = false;
            pdfviewer.ToolbarSettings.ShowFileTools = false;
            pdfviewer.ZoomMode = ZoomMode.FitWidth;
        }

        private async void GridBusqueda_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (gridBusqueda.SelectedIndex != -1 )
            {
                await UpdateSelectedDocumentAsync();
            }
        }

        /// <summary>
        /// Actualiza el documento seleccionado.
        /// </summary>
        private async Task UpdateSelectedDocumentAsync()
        {
            _pages?.Clear();
            try
            {
                if (gridBusqueda.SelectedIndex != -1)
                {
                    _selectedDocument = _documentos[gridBusqueda.SelectedIndex];

                    if (_loadImagesToken != null && _loadImagesToken.CanBeCanceled)
                    {
                        tokenSource.Cancel();
                    }
                    tokenSource = new CancellationTokenSource();
                    _loadImagesToken = tokenSource.Token;

                    await UpdatedGridSelectionAsync(_loadImagesToken);
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        private void LbxPdfImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxPdfImages.SelectedIndex != -1)
            {
                UpdateSelectedImage(Convert.ToInt32(_pages[lbxPdfImages.SelectedIndex].Index)-1); //se le pone más uno porque el litsview inicia de 0 y el index de las imágenes desde 1
                controlTabConsulta.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// Actualiza la imagen seleccionada. 
        /// </summary>
        /// <param name="selectedIndex">Index de la imagen seleccionada. -1 por defecto.</param>
        private void UpdateSelectedImage(int selectedIndex = -1)
        {
            pdfviewer.CurrentPage = selectedIndex + 1;
            //var image = _loadedDocument.Pages[selectedIndex].ExtractImages().FirstOrDefault();
            //if (image != null)
            //{
            //    imageViewerConsulta.Image?.Dispose();
            //    imageViewerConsulta.Image = image.ToImageSource().StreamFromBitmapSource();
            //    image.Dispose();
            //}
        }

        /// <summary>
        /// Actualiza la lista de imágenes cuando el index del grid cambia.
        /// </summary>
        private async Task UpdatedGridSelectionAsync(CancellationToken _loadImagesToken)
        {
            _pdfName = GlobalClass.ruta_proyecto + $@"/{_selectedDocument.Lote.id}/{_selectedDocument.Caja}/{_selectedDocument.NumExpediente}/{_selectedDocument.NumExpediente}.pdf";
            if (!File.Exists(_pdfName)) _pdfName = GlobalClass.ruta_proyecto + $@"/{_selectedDocument.Lote.nom_lote}/{_selectedDocument.Caja}/{_selectedDocument.NumExpediente}.pdf";

            if (File.Exists(_pdfName))
            {
                try
                {
                    pdfviewer.Load(_pdfName);
                    _pdfViewer = new PdfViewerControl();

                    _loadedDocument = new PdfLoadedDocument(_pdfName);  //carga el documento desde el disco
                    _pdfViewer.Load(loadedDocument: _loadedDocument);
                    int indexDoc = 1;
                    foreach (PdfPageBase page in _loadedDocument.Pages) //Carga en _pages cada una de las páginas
                    {
                        if (!_loadImagesToken.IsCancellationRequested &&
                            _selectedDocument.PagIni <= indexDoc &&
                            indexDoc <= _selectedDocument.PagFin)
                        {
                            BitmapSource image = await Task.Run(() => _pdfViewer.ExportAsImage(indexDoc - 1, customSize: new SizeF(150f, 150f), true));
                            _pages.Add(new PDFPages
                            {
                                Source = image,
                                Index = indexDoc.ToString(),
                                IndexOld = indexDoc.ToString()
                            });
                        }
                        indexDoc++;
                    }
                    if (!_loadImagesToken.IsCancellationRequested)
                    {
                        UpdateSelectedImage();
                    }
                    else
                    {
                        _pages.Clear();
                    }

                }
                catch (Exception)
                {
                }
            }
            else
            {
                MessageBox.Show("No se ha encontrado el archivo: \n " + _pdfName);
            }
        }
        private void ExpanderResultados_Expanded(object sender, RoutedEventArgs e)
        {
            consultaExpander.IsExpanded = false;
        }

        private void ConsultaExpander_Expanded(object sender, RoutedEventArgs e)
        {
            expanderResultados.IsExpanded = false;
        }

        private async void BusquedaVozView_Loaded(object sender, RoutedEventArgs e)
        {
            _tiposDoc = await Task.Run(() => EntitiesRepository.Entities.p_tipodoc.AsNoTracking().Select(x => x.nombre).OrderBy(x => x).ToList());
            // cbx_TipoDocumento.ItemsSource = _tiposDoc;
            documentosFilter.AutoCompleteSource = _tiposDoc;
        }

        private void btnConsultarDocumental_Click(object sender, RoutedEventArgs e)
        {
            //string fud = txt_Fud.Text;
            string codCaja = txt_CodCaja_parse.Text;
            string nombre = txt_Nombre.Text;
            string apellido = txt_PrimerApellido.Text;
            string carpeta = txt_carpeta_parse.Text;
            string identificacion = txt_noIndetificacion_numerico.Text;


            string subdependencia = txt_subdependencia.Text;
            if (!string.IsNullOrWhiteSpace(subdependencia))
            {
                try
                {
                    int value = Convert.ToInt32(subdependencia);
                    if (value < 0)
                    {
                        MessageBox.Show("Subdependencia no puede contener valores negativos.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    MessageBox.Show("Subdependencia debe ser un valor numérico entero.");
                    return;
                }
            }


            string subserie = txt_subserie.Text;

            if (!string.IsNullOrWhiteSpace(subserie))
            {
                try
                {
                    int value = Convert.ToInt32(subserie);
                    if (value < 0)
                    {
                        MessageBox.Show("Subserie no puede contener valores negativos.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    MessageBox.Show("Subserie debe ser un valor numérico entero.");
                    return;
                }
            }

            string lote = txt_Lote.Text;
            string expedicion = txt_lugarExpedicion.Text;
            bool incluirSolicitantes = chk_Solicitantes.IsEnabled;
            bool soloTitulares = chk_Titulares.IsEnabled;

            IQueryable<t_documento> rootQuery = GenerateRootQuery();
            if (incluirSolicitantes)
            {
                rootQuery = rootQuery.Include("t_tercero.t_documento_tercero.t_tercero");
            }
            //if (!string.IsNullOrWhiteSpace(fud))
            //{

            //}
            if (!string.IsNullOrWhiteSpace(identificacion))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_tercero.identificacion.ToUpper() == identificacion.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(codCaja))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.nro_caja.ToUpper() == codCaja.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_tercero.nombres.ToUpper().Contains(nombre.ToUpper()));
            }
            if (!string.IsNullOrWhiteSpace(apellido))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_tercero.apellidos.ToUpper().Contains(apellido.ToUpper()));
            }
            if (!string.IsNullOrWhiteSpace(carpeta))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.nro_expediente.ToUpper() == carpeta.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(subdependencia))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_lote.p_subdependencia.nombre == subdependencia);
            }
            if (!string.IsNullOrWhiteSpace(subserie))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_lote.p_subserie.nombre == subserie);
            }
            if (!string.IsNullOrWhiteSpace(lote))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_lote.nom_lote == lote);
            }
            if (!string.IsNullOrWhiteSpace(expedicion))
            {
                rootQuery = rootQuery.Where(x => x.t_carpeta.t_tercero.lugar_exp.ToUpper().Contains(expedicion.ToUpper()));
            }
            List<string> tiposDoc = new List<string>();
            if (documentosFilter.SelectedItem != null)
            {
                foreach (var item in documentosFilter.SelectedItem as IEnumerable)
                {
                    tiposDoc.Add(item.ToString());
                }
            }


            if (tiposDoc.Count != 0)
            {
                rootQuery = rootQuery.Where(x => tiposDoc.Any(y => y.Contains(x.p_tipodoc.nombre)));
            }


            _documentos = rootQuery.Select(x => new BusquedaModel
            {
                Documento = x.t_carpeta.t_tercero.identificacion,
                FolioInicial = x.pag_ini.Value,
                Nombre = x.t_carpeta.t_tercero.nombres,
                TipoDocumental = x.p_tipodoc.nombre,
                Expediente = x.t_carpeta.nom_expediente,
                Caja = x.t_carpeta.nro_caja,
                Lote = x.t_carpeta.t_lote,
                NumExpediente = x.t_carpeta.nro_expediente,
                PagIni = x.pag_ini,
                PagFin = x.pag_fin
            }).ToList();
            gridBusqueda.ItemsSource = _documentos;
            consultaExpander.IsExpanded = false;
            expanderResultados.IsExpanded = true;
        }

        private IQueryable<t_documento> GenerateRootQuery()
        {
            var rootQuery = EntitiesRepository.Entities.t_documento.Include("p_tipodoc.p_tipoitem.p_tiporesp").Include("p_tipodoc").Include("t_carpeta.t_lote.p_subserie").Include("t_carpeta.t_lote.p_subdependencia").Include("t_carpeta.t_tercero").AsNoTracking().Where(x => x.t_carpeta.t_lote.id_proyecto == GlobalClass.id_proyecto).AsQueryable();
            return rootQuery;
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
            else if ((e.Key == Key.LeftShift) && _alreadyDown)
            {
                _alreadyDown = false;
                StaticDeepSpeech.StopRecording();
                if (StaticDeepSpeech.Result != null)
                {
                    string result = StaticDeepSpeech.Result.Transcription;
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        ParseResult(result);
                    }
                }
            }
        }

        private void ParseResult(string result)
        {
            result = result.ToLower();
            if (result.ToLower().Contains("número de identificación"))
            {
                txt_noIndetificacion_numerico.Text = result.ToLower().Replace("número de identificación", "").ToNumber().ToUpper();
            }
            else if (result.ToLower().Contains("código caja") || result.ToLower().Contains("código de caja"))
            {
                txt_CodCaja_parse.Text = result.ToLower().Replace("código caja", "").ParseToChar().ToUpper();
            }
            else if (result.ToLower().Contains("nombre"))
            {
                txt_Nombre.Text = result.ToLower().Replace("nombre", "").ToUpper();
            }
            else if (result.ToLower().Contains("primer apellido"))
            {
                txt_PrimerApellido.Text = result.ToLower().Replace("primer apellido", "").ToUpper();
            }
            else if (result.ToLower().Contains("carpeta"))
            {
                txt_carpeta_parse.Text = result.ToLower().Replace("carpeta", "").ParseToChar().ToUpper();
            }
            else if (result.ToLower().Contains("subdependecia"))
            {
                txt_subdependencia.Text = result.ToLower().Replace("subdependencia", "").ToUpper();
            }
            else if (result.ToLower().Contains("subserie"))
            {
                txt_subserie.Text = result.ToLower().Replace("subserie", "").ToUpper();
            }
            else if (result.ToLower().Contains("lote"))
            {
                txt_Lote.Text = result.ToLower().Replace("lote", "").ToNumber().ToUpper();
            }
            else if (result.ToLower().Contains("lugar expedición"))
            {
                txt_lugarExpedicion.Text = result.ToLower().Replace("lugar expedición", "").ToUpper();
            }
            else if (result.ToLower().Contains("incluir solicitantes"))
            {
                chk_Titulares.IsChecked = false;
                chk_Solicitantes.IsChecked = !chk_Solicitantes.IsChecked.Value;
            }
            else if (result.ToLower().Contains("solo titulares"))
            {
                chk_Titulares.IsChecked = !chk_Titulares.IsChecked.Value;

                if (chk_Titulares.IsChecked.Value)
                {
                    chk_Solicitantes.IsChecked = false;
                }
            }
            StaticDeepSpeech.Result = null;
        }

        private void IndexLote_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftAlt || e.Key == Key.System) && !_alreadyDown)
            {
                Console.WriteLine("Key down ctrl");
                EnableSpeechRecognition();
            }
            else if ((e.Key == Key.LeftShift) && !_alreadyDown)
            {
                StaticDeepSpeech.SetFocus(null, Dispatcher);
                Console.WriteLine("Key down sft");
                EnableSpeechRecognition();
            }
        }

        private void EnableSpeechRecognition()
        {
            _alreadyDown = true;
            StaticDeepSpeech.StartRecording();
        }

        private void cbxTipoConsulta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txt_CodCaja_parse.Text = string.Empty;
            txt_Nombre.Text = string.Empty;
            txt_PrimerApellido.Text = string.Empty;
            txt_carpeta_parse.Text = string.Empty;
            txt_noIndetificacion_numerico.Text = string.Empty;
            txt_subdependencia.Text = string.Empty;
            txt_subserie.Text = string.Empty;
            txt_Lote.Text = string.Empty;
            txt_lugarExpedicion.Text = string.Empty;
            chk_Solicitantes.IsChecked = false;
            chk_Titulares.IsChecked = false;
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            using (var fileDialog = new System.Windows.Forms.SaveFileDialog() { FilterIndex = 2, Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx" })
            {
                var excelEngine = gridBusqueda.ExportToExcel(gridBusqueda.View, new ExcelExportingOptions {  });
                var workBook = excelEngine.Excel.Workbooks[0];
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (fileDialog.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;
                    else if (fileDialog.FilterIndex == 2)
                        workBook.Version = ExcelVersion.Excel2010;
                    else
                        workBook.Version = ExcelVersion.Excel2013;
                    workBook.SaveAs($"{fileDialog.FileName}");
                    MessageBox.Show("Archivo exportado.");
                }
            }
        }
    }
}
