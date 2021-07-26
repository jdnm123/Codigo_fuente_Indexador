using CSCore.CoreAudioAPI;
using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Models;
using Indexai.Services;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Syncfusion.Data;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Z.EntityFramework.Plus;
using Indexai.Extensions;
using Indexai.Helpers;

namespace Indexai.Views
{

    /// <summary>
    /// Interaction logic for IndexadorView.xaml
    /// </summary>
    public partial class IndexadorView : UserControl
    {
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private ObservableCollection<PDFPages> _pages = new ObservableCollection<PDFPages>(new List<PDFPages>());
        private ObservableCollection<PDFPages> _pagesTEMP = new ObservableCollection<PDFPages>(new List<PDFPages>());
        private List<LockedPages> PagBloqueadas = new List<LockedPages>();
        //private List<PDFPages> _nonObservPages = new List<PDFPages>();

        private ObservableCollection<CarpetaModel> _misLotes;
        private ObservableCollection<CarpetaModel> _lotesPublicos;

        private CarpetaModel _selectedItem;
        private CancellationToken _loadImagesToken;

        private DatosBasicosLoteWindow _datosBasicosLote;
        private const string ConfigurationFile = "micro-config.json";

        private ConfiguracionMicro _microWindow;
        private int _publicosStartIndex = 0;
        private int _userStartIndex = 0;

        private int _threadSafeBoolBackValue = 0;

        private string rutaAddReplacePDF;
        private int numPaginasAddReplace = 0;

        private int LastItemSelectedUser = -2;
        private int LastItemSelectedPublic = -2;

        internal void DisableIndex()
        {
            btnIndexar.IsEnabled = false;
            btnDatosLote.IsEnabled = false;
            indexadorGridPublicos.SelectedIndex = -1;
            indexadorGridUser.SelectedIndex = -1;
            _currentItem = null;
        }

        /// <summary>
        /// Cancela el indexado
        /// </summary>
        internal void CancelIndex()
        {
            indexDocumentos.CleanInputs();
            /*foreach (var img in _pages)
            {
                img.Dispose();
            }*/
            _pages.Clear();
            lbxPdfImages.ItemsSource = null;
            lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
            indexDocumentos.Visibility = Visibility.Hidden;
            nestedTabController.SelectedIndex = 0;
        }

        internal void SetCurrentNull()
        {
            _currentItem = null;
        }

        /// <summary>
        /// Threadsafe lock para la carca de imágenes.
        /// </summary>
        public bool IsLoadingImages
        {
            get => (Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 1) == 1);
            set
            {
                if (value) Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 0);
                else Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 0, 1);
            }
        }

        /// <summary>
        /// Selecciona la carpeta que estaba en indexado anteriormente.
        /// </summary>
        internal void SetPublicIndex()
        {
            indexadorGridUser.SelectedIndex = GlobalClass.UserSelectedIndex;
            btnIndexar.IsEnabled = false;
            Indexar();
        }

        /// <summary>
        /// Indica la selección actual del grid para no recargar si selecciona el mismo item.
        /// </summary>
        private CarpetaModel _currentItem = null;

        /// <summary>
        /// Libera el stream de la imagen actual.
        /// </summary>
        internal void ReleaseImageStream()
        {
        }

        private bool pdfCargado = true;
        private DispatcherTimer timer = new DispatcherTimer();
        
        //private PdfLoadedDocument _loadedDocument;
        private string _ruta;
        private int _selectedImageIndex = -1;
        private bool _pdfViewerLoaded;
        /// <summary>
        /// Elementos a liberar.
        /// </summary>
        private ObservableCollection<object> _selectedItemsUser;
        private ObservableCollection<object> _selectedItemsPublicos;
        private int _publicosSelectedIndex = -1;
        private int _userSelectedIndex = -1;

        /// <summary>
        /// Indica si permitir la recarga del PDF en el view.
        /// </summary>
        private bool _enableReloadPdf = true;
        private Syncfusion.UI.Xaml.Grid.GridFilterEventArgs _publicosFilters;
        private IList<Syncfusion.UI.Xaml.Grid.SortColumnDescription> _publicSortItems;

        public IndexadorView()
        {
            InitializeComponent();
            Loaded += IndexadorView_Loaded;
            lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
            pdfviewer.PageRemoved += Pdfviewer_PageRemoved;
            pdfviewer.PageRotated += Pdfviewer_PageRotated;
            pdfviewer.DocumentLoaded += Pdfviewer_DocumentLoaded;
            pdfviewer.ErrorOccurred += Pdfviewer_ErrorOccurred;
            indexadorGridPublicos.FilterChanged += IndexadorGridPublicos_FilterChanged;
            indexadorGridPublicos.SortColumnsChanged += IndexadorGridPublicos_SortColumnsChanged;
            //pdfviewer
            /*imageViewer.ToolbarSettings.ToolbarItemSelected += ToolbarSettings_ToolbarItemSelected;
            imageViewer.ToolbarSettings.IsToolbarVisiblity = false;
            imageViewer.ImageSaving += ImageViewer_ImageSaving;*/

            _microWindow = new ConfiguracionMicro();
            _microWindow.Closed += _microWindow_Closed;

            LoadDeepSpeechConfig();
            //registro eventos de los grids
            indexadorGridPublicPager.OnDemandLoading += IndexadorPublicPager_OnDemandLoading;
            indexadorGridUserPager.OnDemandLoading += IndexadorGridUserPager_OnDemandLoading;
            indexadorGridPublicPager.PageIndexChanged += IndexadorGridPublicPager_PageIndexChanged;
            indexadorGridUserPager.PageIndexChanged += IndexadorGridUserPager_PageIndexChanged;
            lotesTab.SelectedIndexChanged += LotesTab_SelectedIndexChanged;

            dlgAddImg.DialogClosing += dlgAddImg_OnDialogClosing;
            dlgDelImg.DialogOpened += dlgDelImg_OnDialogOpened;
            dlgDelImg.DialogClosing += dlgDelImg_OnDialogClosing;
            dlgRotateImg.DialogOpened += dlgRotateImg_OnDialogOpened;
            dlgRotateImg.DialogClosing += dlgRotateImg_OnDialogClosing;
            dlgAddReplacePDF.DialogOpened += dlgAddReplacePDF_OnDialogOpened;
            dlgAddReplacePDF.DialogClosing += dlgAddReplacePDF_OnDialogClosing;
            cbopcionAddReplaceImg.DropDownClosed += cbopcionAddReplaceImg_DropDownClosed;
            cbAnglerotateImg.Items.Clear();
            cbAnglerotateImg.Items.Add(90);
            cbAnglerotateImg.Items.Add(180);
            cbAnglerotateImg.Items.Add(270);
            cbopcionAddReplaceImg.Items.Clear();
            cbopcionAddReplaceImg.Items.Add("Adicionar");
            cbopcionAddReplaceImg.Items.Add("Remplazar");
            btnAddImage.Visibility = Visibility.Hidden;
            btnDeleteImage.Visibility = Visibility.Hidden;
            btnRotateImage.Visibility = Visibility.Hidden;
            btnAddReplacePDF.Visibility = Visibility.Hidden;
            btnOpenWorkingFolder.Visibility = Visibility.Hidden;
            btnDeleteImage.IsEnabled = true;
            btnDatosLote.IsEnabled = false;
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += intervalo_CargaPDF;
            IsLoadingImages = false;
            pdfviewer.ToolbarSettings.ShowAnnotationTools = false;
            pdfviewer.ToolbarSettings.ShowFileTools = false;
            pdfviewer.ZoomMode = ZoomMode.FitWidth;
            pdfviewer.WarnBeforeClose = false;
            string dirEXE = System.IO.Path.GetFullPath(".\\");
            if (File.Exists(dirEXE+"/logo_" + GlobalClass.id_proyecto + ".png"))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dirEXE + "/logo_" + GlobalClass.id_proyecto + ".png", UriKind.Absolute);
                bitmap.EndInit();
                ImgLogo.Source = bitmap;
            }

            //indexadorGridPublicos.Columns[0].filters.Add(new FilterPredicate { FilterValue = 5, FilterMode=ColumnFilter.Value, FilterType=FilterType.Equals });
        }

        private void IndexadorGridPublicos_SortColumnsChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSortColumnsChangedEventArgs e)
        {
            _publicSortItems = e.AddedItems;
            RefreshPublicos(_publicosStartIndex);
        }

        private void IndexadorGridPublicos_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {
            _publicosFilters = e;
            RefreshPublicos(_publicosStartIndex);
        }

        private void dlgAddReplacePDF_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            string opcion = String.Empty;
            if (!Equals(eventArgs.Parameter, true)) return;
            if (string.IsNullOrEmpty(cbopcionAddReplaceImg.Text))
            {
                MessageBox.Show("Debe escoger la opción que puede ser adicionar o Reemplazar", "Alpha AI");
                eventArgs.Cancel();
                return;
            }
            if (string.IsNullOrEmpty(cbpositionAddReplaceImg.Text))
            {
                MessageBox.Show("Debe Seleccionar la página desde la que desea " + cbopcionAddReplaceImg.Text, "Alpha AI");
                eventArgs.Cancel();
                return;
            }
            if (string.IsNullOrEmpty(txtAddReplaceImg.Text))
            {
                MessageBox.Show("Debe escribir una respuesta", "Alpha AI");
                txtAddReplaceImg.Text = string.Empty;
                eventArgs.Cancel();
                return;
            }

            try
            {
                int _pos = 1;
                _pos = GlobalClass.GetNumber(cbpositionAddReplaceImg.Text, _pos);    //int.TryParse(cbpositionNewImg.Text, out _pos);
                _pos--;
                if (_pages.Count > 0)
                {
                    ArchivoMeta metaFile = PDFHelper.CopiaPdf(_selectedItem);  //Hace una copia del archivo Original
                    _ruta = metaFile.RutaFuente;
                    string pdfOrigen = metaFile.RutaFuente; //Ruta Original
                    string pdfCopia = metaFile.RutaCopia;   //Ruta Copia

                    PdfLoadedDocument oldPdfDoc = new PdfLoadedDocument(pdfCopia); //Abre PDF Copia
                    PdfLoadedDocument appPdfDoc = new PdfLoadedDocument(rutaAddReplacePDF); //Abre PDF a Adicionar
                    int TotalPag = oldPdfDoc.Pages.Count;   //Total páginas PDF Copia
                    PdfDocument doc = new PdfDocument();    //PDF Nuevo
                    if (_pos > 0) doc.ImportPageRange(oldPdfDoc, 0, _pos - 1);  //Páginas Iniciales para el PDF nuevo (Antes de Add o Replace)
                    doc.ImportPageRange(appPdfDoc, 0, numPaginasAddReplace - 1);    //adiciona páginas del PDF Nuevo
                    if (cbopcionAddReplaceImg.Text == "Adicionar")
                    {
                        if (_pos < TotalPag) doc.ImportPageRange(oldPdfDoc, _pos, TotalPag - 1); //Importa las pags
                        doc.Save(_ruta); //Guarda en la ubicación original
                        doc.Close(true); //Cierra PDF Nuevo
                        oldPdfDoc.Close(true);  //Cierra el PDF copia
                        //Extrae las imágenes del PDF adicionado
                        //List<System.Drawing.Image> listImgs = new List<System.Drawing.Image>();
                        for (int p = 0; p < numPaginasAddReplace; p++)
                        {
                            System.Drawing.Image imagen = ImageExtensions.DrawText("NA", Color.Black, Color.White);
                            System.Drawing.Image imagenPDF = appPdfDoc.Pages[p].ExtractImages().FirstOrDefault();
                            if (imagenPDF != null) imagen = imagenPDF;
                            //listImgs.Add(imagen);
                            _pages.Insert((_pos + p), new PDFPages
                            {
                                Source = ImageExtensions.ToImageSource(imagen),
                                Index = (_pos + p + 1).ToString(),
                                IndexOld = (_pos + p + 1).ToString()
                            });
                        }
                        //Reorganiza las páginas siguientes para la vista en miniatura LISTVIEW
                        int ini_pages = _pos + numPaginasAddReplace;
                        int fin_pages = _pages.Count;
                        for (int p = ini_pages; p < fin_pages; p++)
                        {
                            _pages[p].Index = (p + 1).ToString();
                            _pages[p].IndexOld = (p + 1).ToString();
                        }
                        appPdfDoc.Close(true);

                        //Si hay indexación en las páginas siguientes recalcula las páginas
                        List<t_documento> res = EntitiesRepository.Entities.t_documento.Where(r => r.id_carpeta == _selectedItem.IdCarptera && r.pag_ini > _pos).ToList();
                        // update
                        foreach (var r in res)
                        {
                            r.pag_ini = r.pag_ini + numPaginasAddReplace;
                            r.pag_fin = r.pag_fin + numPaginasAddReplace;
                        }
                        EntitiesRepository.Entities.SaveChanges();// save

                        //Recarga PDF
                        ReloadPdf(_ruta);
                        lbxPdfImages.ItemsSource = null;
                        activaIndexacion();
                        UpdateSelectedImage(_pos);
                    }
                    else
                    {   //Remplazar
                        if (_pos < TotalPag) doc.ImportPageRange(oldPdfDoc, _pos + numPaginasAddReplace, TotalPag - 1);    //Importa las pags excluyendo las que se reemplazan
                        doc.Save(_ruta);    //Guarda en la ubicación original
                        doc.Close(true);    //Cierra PDF Nuevo
                        oldPdfDoc.Close(true);  //Cierra el PDF copia
                        //Extrae las imágenes del PDF adicionado
                        //List<System.Drawing.Image> listImgs = new List<System.Drawing.Image>();
                        for (int p = 0; p < numPaginasAddReplace; p++)
                        {
                            if(_pages.Count >= _pos + p)
                            {
                                System.Drawing.Image imagen = ImageExtensions.DrawText("NA", Color.Black, Color.White);
                                System.Drawing.Image imagenPDF = appPdfDoc.Pages[p].ExtractImages().FirstOrDefault();
                                if (imagenPDF != null) imagen = imagenPDF;
                                //Actualiza las imágenes en el Listview
                                //listImgs.Add(imagen);
                                _pages[_pos + p].Source = imagen.ToImageSource();
                            }
                        }
                        appPdfDoc.Close(true);
                        //Recarga PDF
                        ReloadPdf(_ruta);
                        lbxPdfImages.ItemsSource = null;
                        OcultaImgIndexadas();
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("El pdf está en uso, es necesario cerrarlo para poder modificar las páginas.");
            }
        }

        private void btnAddReplacePDF_Click(object sender, RoutedEventArgs e)
        {
            if (_pages.Count > 0)
            {
                System.Windows.Forms.OpenFileDialog opf = new System.Windows.Forms.OpenFileDialog();
                opf.Filter = "PDF File (*.pdf)|*.pdf";
                if (opf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    rutaAddReplacePDF = opf.FileName;
                    dlgAddReplacePDF.IsOpen = true;
                    cbpositionAddReplaceImg.SelectedItem = GlobalClass.selPagFinal;
                }
                else
                {
                    MessageBox.Show("Primero debe cargar un documento", "Alpha AI");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Primero debe cargar un documento", "Alpha AI");
                return;
            }
        }

        private void dlgAddReplacePDF_OnDialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            PdfLoadedDocument newPdfDoc = new PdfLoadedDocument(rutaAddReplacePDF);
            numPaginasAddReplace = newPdfDoc.Pages.Count;
            numImagenes.Text = $"Imágenes a Adicionar / Reemplazar: {numPaginasAddReplace}";
            newPdfDoc.Close();
        }

        private void dlgRotateImg_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            if (string.IsNullOrEmpty(cbpositionRotateImg.Text))
            {
                MessageBox.Show("Debe Seleccionar la página a rotar", "Alpha AI");
                cbpositionRotateImg.Text = string.Empty;
                eventArgs.Cancel();
            }
            else if (string.IsNullOrEmpty(cbAnglerotateImg.Text))
            {
                MessageBox.Show("Debe escoger un ángulo", "Alpha AI");
                cbAnglerotateImg.Text = string.Empty;
                eventArgs.Cancel();
            }
            else
            {
                try
                {
                    int _pos = 1;
                    _pos = GlobalClass.GetNumber(cbpositionRotateImg.Text, _pos);    //int.TryParse(cbpositionNewImg.Text, out _pos);
                    _pos--;
                    if (_pages.Count > 0)
                    {
                        ArchivoMeta metaFile = PDFHelper.CopiaPdf(_selectedItem);
                        _ruta = metaFile.RutaFuente;
                        string pdfOrigen = metaFile.RutaFuente;
                        string pdfCopia = metaFile.RutaCopia;

                        PdfLoadedDocument oldPdfDoc = new PdfLoadedDocument(pdfCopia);
                        PdfLoadedPage pagBase = oldPdfDoc.Pages[_pos] as PdfLoadedPage;
                        SizeF pageSize = pagBase.Size;
                        System.Drawing.Image imagen = oldPdfDoc.Pages[_pos].ExtractImages().FirstOrDefault();

                        if (imagen != null)
                        {
                            if (cbAnglerotateImg.Text == "90") imagen.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            if (cbAnglerotateImg.Text == "180") imagen.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            if (cbAnglerotateImg.Text == "270") imagen.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            imagen.Save("rotada.jpeg", ImageFormat.Jpeg);
                            PdfExportService _exportService = new PdfExportService();
                            PdfPage pagNueva = _exportService.Imagen2Pdf("rotada.pdf", "rotada.jpeg", imagen, pageSize);
                            PdfLoadedDocument docRotado = new PdfLoadedDocument("rotada.pdf");
                            oldPdfDoc.Pages.RemoveAt(_pos);
                            //oldPdfDoc.Split("tmp");
                            int TotalPag = oldPdfDoc.Pages.Count;
                            PdfDocument doc = new PdfDocument();
                            if (_pos > 0) doc.ImportPageRange(oldPdfDoc, 0, _pos - 1);
                            doc.ImportPage(docRotado, 0);
                            if (_pos < TotalPag) doc.ImportPageRange(oldPdfDoc, _pos, TotalPag - 1);
                            doc.Save(_ruta);
                            docRotado.Close(true);
                            doc.Close(true);
                            oldPdfDoc.Close(true);
                            ReloadPdf(_ruta);
                            _pages[_pos].Source = imagen.ToImageSource();
                            lbxPdfImages.ItemsSource = null;
                            OcultaImgIndexadas();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró la imágen en el página:" + _pos);
                        }
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("El pdf está en uso, es necesario cerrarlo para poder girar páginas.");
                }
            }
        }

        private void cbopcionAddReplaceImg_DropDownClosed(object sender, EventArgs e)
        {
            string txt = "Adicionar";
            if (cbopcionAddReplaceImg.Text != "Adicionar") txt = "Remplazar";
            numImagenes.Text = $"Imágenes a {txt}: {numPaginasAddReplace}";
        }

        private void Pdfviewer_ErrorOccurred(object sender, ErrorOccurredEventArgs args)
        {
        }

        private void Pdfviewer_DocumentLoaded(object sender, EventArgs args)
        {
            if (_selectedImageIndex != -1)
            {
                pdfviewer.CurrentPage = _selectedImageIndex;
            }
            _pdfViewerLoaded = true;
            CacheHelper.SaveToCacheAsync(pdfviewer, Dispatcher);
        }

        private async void Pdfviewer_PageRemoved(object sender, PageRemovedEventArgs e)
        {
            try
            {
                pdfviewer.GoToFirstPage();
                _pdfViewerLoaded = false;
                await pdfviewer.LoadAsync(_ruta);
            }
            catch (PdfException)
            {
                MessageBox.Show($"PDF corrupto, error al cargar: {_ruta}");
            }
        }

        private async void Pdfviewer_PageRotated(object sender, PageRotatedEventArgs e)
        {
            try
            {
                pdfviewer.GoToFirstPage();
                _pdfViewerLoaded = false;
                await pdfviewer.LoadAsync(_ruta);
            }
            catch (PdfException)
            {
                MessageBox.Show($"PDF corrupto, error al cargar: {_ruta}");
            }
        }

        private void cbpositionDelImg_DropDownClosed(object sender, EventArgs e)
        {
            int pag = 0;

            foreach (Indexai.Models.PDFPages item in lbxPdfImages.Items)
            {
                Console.WriteLine(item);
                if (item.Index.Equals(cbpositionDelImg.Text))
                {
                    int.TryParse(item.IndexOld, out pag);
                    SetImageToViewer(pag - 1);
                }
                pag++;
            }
        }

        private void dlgDelImg_OnDialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            cbpositionDelImg.SelectedItem = GlobalClass.selPagFinal;
        }

        private void dlgRotateImg_OnDialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            cbpositionRotateImg.SelectedItem = GlobalClass.selPagFinal;
        }

        internal void UpdateFromAdmin()
        {
            throw new NotImplementedException();
        }

        private void _datosBasicosLote_Closed(object sender, EventArgs e)
        {
            //actualiza las páginas cuando se cierran los datos básicos.
            _datosBasicosLote.Closed -= _datosBasicosLote_Closed;
            UpdatePublicPage(true);
            UpdateUserPage(true);
            UpdateWithReset();
            _enableReloadPdf = false;
            if (lotesTab.SelectedIndex == 0)
            {
                indexadorGridUser.SelectedIndex = _userSelectedIndex;
            }
            else
            {
                indexadorGridPublicos.SelectedIndex = _publicosSelectedIndex;
            }
            _enableReloadPdf = true;
        }

        //Verifica si ya está cargado el PDF, si es así hace la indexacion
        private void activaIndexacion()
        {
            ResetView();
            //_datosBasicosLote = new DatosBasicosLoteWindow();
            //btnAddImage.Visibility = Visibility.Visible;
            btnOpenWorkingFolder.Visibility = Visibility.Visible;
            if (GlobalClass.loc_admin == 1 || GlobalClass.loc_calidad == 1 || GlobalClass.loc_index == 1)
            {
                btnDeleteImage.Visibility = Visibility.Visible;
                btnRotateImage.Visibility = Visibility.Visible;
                btnAddReplacePDF.Visibility = Visibility.Visible;
            }

            if (GlobalClass.Carpeta == null) return;
            //pdfviewer
            //imageViewer.ToolbarSettings.IsToolbarVisiblity = true;
            indexDocumentos.Visibility = Visibility.Visible;

            indexDocumentos.LoadData();
            PaginasPendientes();
            indexDocumentos.SetIndexadorView(this);
            Dispatcher.BeginInvoke((Action)(() => nestedTabController.SelectedIndex = 1));
        }

        private void intervalo_CargaPDF(object sender, EventArgs e)
        {
            if (pdfCargado)
            {   //Revisa cada 100 Milisegundos si ya está cargado el pdf para comenzar la indexación
                timer.Stop();
                btnIndexar.IsEnabled = true;
                btnEditar.IsEnabled = true;
                activaIndexacion();
            }
        }

        private void IndexadorGridUserPager_PageIndexChanged(object sender, PageIndexChangedEventArgs e)
        {
            _currentItem = null;
        }

        private void IndexadorGridPublicPager_PageIndexChanged(object sender, PageIndexChangedEventArgs e)
        {
            _currentItem = null;
        }

        private void LotesTab_SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToInt32(e.NewValue) == 1)
            {
                btnIndexar.IsEnabled = false;
                btnDatosLote.IsEnabled = false;
            }
            _currentItem = null;
            btnTomarLote.IsEnabled = false;
            btnLiberarLote.IsEnabled = false;
        }

        private async void dlgDelImg_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            if (string.IsNullOrEmpty(cbpositionDelImg.Text))
            {
                MessageBox.Show("Debe Seleccionar la página a eliminar", "Alpha AI");
                cbpositionDelImg.Text = string.Empty;
                eventArgs.Cancel();
                return;
            }
            else if (string.IsNullOrEmpty(txtDelImg.Text))
            {
                MessageBox.Show("Debe escribir una respuesta", "Alpha AI");
                txtDelImg.Text = string.Empty;
                eventArgs.Cancel();
                return;
            }
            else
            {
                try
                {
                    int _pos = 1;
                    _pos = GlobalClass.GetNumber(cbpositionDelImg.Text, _pos);    //int.TryParse(cbpositionNewImg.Text, out _pos);
                    await RemoverPaginaPdfAsync(_pos, txtDelImg.Text);  //ELIMINA PÁGINA
                    if (_pages.Count > 0)                               //RECALCULA PDFPAGES
                    {
                        ObservableCollection<PDFPages> _pagesTMP = new ObservableCollection<PDFPages>(new List<PDFPages>());
                        for (int i = 0; i < _pages.Count; i++)
                        {
                            if (i + 1 != _pos)
                            {
                                int indexVal = i;
                                if (i + 1 > _pos) indexVal--;
                                _pages[i].Index = (indexVal + 1).ToString();
                                _pages[i].IndexOld = (indexVal + 1).ToString();
                                _pagesTMP.Add(_pages[i]);
                            }
                        }
                        _pages = _pagesTMP;
                        lbxPdfImages.ItemsSource = null;
                    }
                    OcultaImgIndexadas();                           //OCULTA IMAGEN ELIMINADA
                }
                catch (IOException)
                {
                    MessageBox.Show("El pdf está en uso, es necesario cerrarlo para poder Eliminar la página.");
                }
            }
        }

        private void dlgAddImg_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {   //Console.WriteLine(positionNewImg.Text);}
            if (!Equals(eventArgs.Parameter, true)) return;

            if (string.IsNullOrEmpty(cbpositionNewImg.Text))
            {
                MessageBox.Show("Debe ingresar un número de posición", "Alpha AI");
                cbpositionNewImg.Text = string.Empty;
                eventArgs.Cancel();
            }
            else
            {
                int _pos = 1, _np = 0;
                _pos = GlobalClass.GetNumber(cbpositionNewImg.Text, _pos);    //int.TryParse(cbpositionNewImg.Text, out _pos);
                if (_pages.Count > 0)
                {
                    System.Windows.Forms.OpenFileDialog opf = new System.Windows.Forms.OpenFileDialog();
                    opf.Filter = "All Images Files (*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif)|*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif" +
                                "|PNG Portable Network Graphics (*.png)|*.png" +
                                "|JPEG File Interchange Format (*.jpg *.jpeg *jfif)|*.jpg;*.jpeg;*.jfif" +
                                "|BMP Windows Bitmap (*.bmp)|*.bmp" +
                                "|TIF Tagged Imaged File Format (*.tif *.tiff)|*.tif;*.tiff" +
                                "|GIF Graphics Interchange Format (*.gif)|*.gif";
                    if (opf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ObservableCollection<PDFPages> _pagesTMP = new ObservableCollection<PDFPages>(new List<PDFPages>());
                        do
                        {
                            if (_np + 1 == _pos)
                            {
                                BitmapSource bSource = new BitmapImage(new Uri(opf.FileName));
                                _pagesTMP.Add(new PDFPages
                                {
                                    ImageData = bSource,
                                    Index = (_np + 1).ToString(),
                                    Source = ImageExtensions.path2Bitmapsource(opf.FileName),
                                    Edited = true
                                });
                            }
                            else
                            {   //Actualiza el indice para que no quede repetido
                                int indice = _np;
                                if (_np + 1 > _pos)
                                {
                                    _pages[(indice - 1)].Index = (indice + 1).ToString();
                                    indice--;
                                }
                                _pagesTMP.Add(_pages[indice]);
                            }
                            _np++;
                        }
                        while (_np <= _pages.Count);
                        _pages = (ObservableCollection<PDFPages>)_pagesTMP.OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
                        lbxPdfImages.ItemsSource = null;
                        SaveNewPdf();
                        OcultaImgIndexadas();
                    }
                    //cbpositionNewImg.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Primero debe cargar un documento", "Alpha AI");
                    return;
                }
            }
        }

        internal void UpdateFromBasicoLote()
        {
            UpdatePublicPage(removeCache: false);
            UpdateUserPage(removeCache: false);
            nestedTabController.SelectedIndex = 1;
        }

        private void IndexadorGridUserPager_OnDemandLoading(object sender, OnDemandLoadingEventArgs e)
        {
            _userStartIndex = e.StartIndex;

            IQueryable<CarpetaModel> query = UpdateUserQuery();
            _misLotes = new ObservableCollection<CarpetaModel>(query.ToList());
            if (_misLotes?.Count != 0 && _userStartIndex != -1)
            {
                int itemsCount = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.estado == "D" && x.idusr_asignado == GlobalClass.id_usuario && x.t_lote.id_proyecto == GlobalClass.id_proyecto).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / indexadorGridUserPager.PageSize);
                indexadorGridUserPager.PageCount = pageCount;
                indexadorGridUserPager.LoadDynamicItems(_userStartIndex, _misLotes);
                //(indexadorGridUserPager.PagedSource as PagedCollectionView)?.ResetCacheForPage(indexadorGridUserPager.PageIndex);
            }
            else if (_misLotes.Count == 0)
            {
                indexadorGridUserPager.PageCount = 0;
                indexadorGridUserPager.LoadDynamicItems(_userStartIndex, _misLotes);
                //(indexadorGridUserPager.PagedSource as PagedCollectionView)?.ResetCacheForPage(indexadorGridUserPager.PageIndex);
            }

            if (lotesTab.SelectedIndex == 0 && _misLotes?.Count != 0)
            {
                btnLiberarLote.IsEnabled = false;
                btnIndexar.IsEnabled = false;
            }
        }

        /// <summary>
        /// Atualiza la páginas del usuario.
        /// </summary>
        /// <param name="removeCache">Elimina el cache, por defecto False.</param>
        private void UpdateUserPage(bool removeCache = false)
        {
            if (indexadorGridUserPager.PagedSource != null)
            {
                IQueryable<CarpetaModel> query = UpdateUserQuery();
                _misLotes = new ObservableCollection<CarpetaModel>(query.ToList());
                int itemsCount = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.estado == "D" && x.idusr_asignado == GlobalClass.id_usuario && x.t_lote.id_proyecto == GlobalClass.id_proyecto).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / indexadorGridUserPager.PageSize);
                indexadorGridUserPager.PageCount = pageCount != 0 ? pageCount : 1;
                if (_misLotes?.Count != 0)
                {
                    indexadorGridUserPager.LoadDynamicItems(_userStartIndex, _misLotes);
                    if (removeCache)
                    {
                        if (indexadorGridUserPager.PageIndex != -1)
                            indexadorGridUserPager.PagedSource?.ResetCacheForPage(indexadorGridUserPager.PageIndex);
                    }
                }
            }
        }

        private void IndexadorPublicPager_OnDemandLoading(object sender, OnDemandLoadingEventArgs e)
        {
            //index público seleccionados.
            _publicosStartIndex = e.StartIndex;
            RefreshPublicos(_publicosStartIndex);
        }

        private void RefreshPublicos(int startIndex)
        {
            IQueryable<CarpetaModel> query = UpdatePublicosQuery();
            _lotesPublicos = new ObservableCollection<CarpetaModel>(query.ToList());
            indexadorGridPublicos.ItemsSource = _lotesPublicos;
            if (_lotesPublicos?.Count != 0 && startIndex != -1)
            {
                int itemsCount = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.estado == "D" && x.idusr_asignado == null && x.t_lote.id_proyecto == GlobalClass.id_proyecto).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / indexadorGridPublicPager.PageSize);

                indexadorGridPublicPager.PageCount = pageCount;
                //indexadorGridPublicPager.LoadDynamicItems(startIndex, new ObservableCollection<int>(new int [itemsCount]));
            }
            else if (_lotesPublicos.Count == 0)
            {
                indexadorGridPublicPager.PageCount = 0;
                //indexadorGridPublicPager.LoadDynamicItems(startIndex, new ObservableCollection<int>(new int[0]));
            }

            if (lotesTab.SelectedIndex == 1 && _lotesPublicos?.Count != 0)
            {
                btnLiberarLote.IsEnabled = false;
                btnIndexar.IsEnabled = false;
            }
        }

        /// <summary>
        /// Obtiene los documentos públicos visibles para el usuario.
        /// </summary>
        /// <returns>IQueryable sin ejecutar.</returns>
        private IQueryable<CarpetaModel> UpdatePublicosQuery()
        {
            IQueryable<t_carpeta> publicQuery = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Where(x => x.t_lote.id_proyecto == GlobalClass.id_proyecto && x.estado == "D" && (x.idusr_asignado == null));
            txtGridTotalPublicitems.Content = "Total registros: " + publicQuery.Count().ToString("###.###.###");

            if (_publicosFilters != null && _publicosFilters.FilterPredicates != null)
            {
                foreach (var filter in _publicosFilters.FilterPredicates)
                {
                    switch (filter.FilterType)
                    {
                        case FilterType.LessThan:
                            break;
                        case FilterType.LessThanOrEqual:
                            break;
                        case FilterType.Equals:
                            publicQuery = AddEquals(publicQuery, filter);
                            break;
                        case FilterType.NotEquals:
                            break;
                        case FilterType.GreaterThanOrEqual:
                            break;
                        case FilterType.GreaterThan:
                            break;
                        case FilterType.StartsWith:
                            break;
                        case FilterType.NotStartsWith:
                            break;
                        case FilterType.EndsWith:
                            break;
                        case FilterType.NotEndsWith:
                            break;
                        case FilterType.Contains:
                            break;
                        case FilterType.NotContains:
                            break;
                        case FilterType.Undefined:
                            break;
                        case FilterType.Between:
                            break;
                        default:
                            break;
                    }
                }
            }

            publicQuery = AddSorts(publicQuery, _publicSortItems);
            if (_publicSortItems == null)
            {
                publicQuery = publicQuery.OrderBy(x => x.nro_caja);
            }

            return publicQuery.Skip(_publicosStartIndex)
                .Take(indexadorGridPublicPager.PageSize).Select(x => new CarpetaModel
                {
                    t_lote = x.t_lote,
                    nro_caja = x.nro_caja,
                    nro_expediente = x.nro_expediente,
                    nom_expediente = x.nom_expediente,
                    Observaciones = x.kp_observacion,
                    nro_carpeta = x.nro_carpeta ?? -1,
                    CarpetaEstado = x.t_carpeta_estado,
                    Folios = x.total_folios ?? -1,
                    Paginas = x.paginas ?? 0,
                    Asignado = x.p_usuario1.usuario,
                    Estado = x.estado,
                    IdUsuario = x.idusr_asignado ?? -1,
                    NoExpediente = x.nro_expediente,
                    IdSubSerie = x.t_lote.id_subserie ?? -1,
                    IdCarptera = x.id,
                    IdTercero = x.id_tercero ?? -1,
                    Beneficiario = x.t_tercero,
                    PagDefecto = x.t_lote.p_subserie.pag_defecto,
                    hc_inicio = x.hc_inicio,
                    hc_fin = x.hc_fin,
                    tomo = x.tomo,
                    tomo_fin = x.tomo_fin
                }).AsQueryable();
        }

        /// <summary>
        /// Añade los filtros equals a el IQueryable.
        /// </summary>
        /// <param name="publicQuery"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<t_carpeta> AddEquals(IQueryable<t_carpeta> publicQuery, FilterPredicate filter)
        {
            switch (ColumnHelper.GetColumn(_publicosFilters.Column.MappingName))
            {
                case GridColumn.Lote:
                    publicQuery = publicQuery.Where(x => x.t_lote.nom_lote == filter.FilterValue.ToString());
                    break;
                case GridColumn.Caja:
                    publicQuery = publicQuery.Where(x => x.nro_caja == filter.FilterValue.ToString());
                    break;
                case GridColumn.NumExpediente:
                    publicQuery = publicQuery.Where(x => x.nro_expediente == filter.FilterValue.ToString());
                    break;
                case GridColumn.Expediente:
                    publicQuery = publicQuery.Where(x => x.nom_expediente == filter.FilterValue.ToString());
                    break;
                case GridColumn.Carpeta:
                    int filterValue = Convert.ToInt32(filter.FilterValue);
                    publicQuery = publicQuery.Where(x => x.nro_carpeta == filterValue);
                    break;
                case GridColumn.Asignado:
                    publicQuery = publicQuery.Where(x => x.p_usuario1.usuario == filter.FilterValue.ToString());
                    break;
                case GridColumn.Null:
                    break;
            }

            return publicQuery;
        }

        /// <summary>
        /// Adjunto al IQueryable la lista de los sorts de las columnas.
        /// </summary>
        /// <param name="iQueryable">IQueryable</param>
        /// <param name="sortItems">Lista de sorts</param>
        /// <returns>IQueryable con los sorts aplicacdos</returns>
        private IQueryable<t_carpeta> AddSorts(IQueryable<t_carpeta> iQueryable,
            IList<Syncfusion.UI.Xaml.Grid.SortColumnDescription> sortItems)
        {
            if (sortItems != null)
            {
                foreach (var sortColum in sortItems) // Añade al IQueryable todos los sorts de las solumnas
                {
                    switch (sortColum.SortDirection)
                    {
                        case ListSortDirection.Ascending:
                            if (sortColum.ColumnName == "Asignado")
                                iQueryable = iQueryable.OrderBy(x => x.p_usuario1.usuario);
                            else if (sortColum.ColumnName == "t_lote.nom_lote") //para sub propiedades se requiere hacerlo manual
                                iQueryable = iQueryable.OrderBy(x => x.t_lote.nom_lote);
                            else
                                iQueryable = iQueryable.OrderBy(sortColum.ColumnName);
                            break;
                        case ListSortDirection.Descending:
                            if (sortColum.ColumnName == "Asignado")
                                iQueryable = iQueryable.OrderByDescending(x => x.p_usuario1.usuario);
                            else if (sortColum.ColumnName == "t_lote.nom_lote") //para sub propiedades se requiere hacerlo manual
                                iQueryable = iQueryable.OrderByDescending(x => x.t_lote.nom_lote);
                            else
                                iQueryable = iQueryable.OrderByDescending(sortColum.ColumnName);
                            break;
                    }
                }
            }
            return iQueryable;
        }

        /// <summary>
        /// Obtiene los documentos asignados al usuario.
        /// </summary>
        /// <returns>IQueryable sin ejecutar.</returns>
        private IQueryable<CarpetaModel> UpdateUserQuery()
        {
            IQueryable<t_carpeta> userQuery = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("t_carpeta_estado").AsNoTracking().Where(x => x.estado == "D" && x.idusr_asignado == GlobalClass.id_usuario && x.t_lote.id_proyecto == GlobalClass.id_proyecto);
            txtGridTotalUseritems.Content = "Total registros: " + userQuery.Count().ToString("###.###.###");
            return userQuery
                .OrderBy(x => x.nro_caja).Skip(_userStartIndex)
                .Take(indexadorGridUserPager.PageSize).Select(x => new CarpetaModel
                {
                    t_lote = x.t_lote,
                    nro_caja = x.nro_caja,
                    nro_expediente = x.nro_expediente,
                    nom_expediente = x.nom_expediente,
                    Observaciones = x.kp_observacion,
                    nro_carpeta = x.nro_carpeta ?? -1,
                    Folios = x.total_folios ?? -1,
                    Paginas = x.paginas ?? 0,
                    CarpetaEstado = x.t_carpeta_estado,
                    Asignado = x.p_usuario1.usuario,
                    Estado = x.estado,
                    IdUsuario = x.idusr_asignado ?? -1,
                    NoExpediente = x.nro_expediente,
                    IdSubSerie = x.t_lote.id_subserie ?? -1,
                    IdCarptera = x.id,
                    IdTercero = x.id_tercero ?? -1,
                    Beneficiario = x.t_tercero,
                    PagDefecto = x.t_lote.p_subserie.pag_defecto,
                    hc_inicio = x.hc_inicio,
                    hc_fin = x.hc_fin,
                    tomo = x.tomo,
                    tomo_fin = x.tomo_fin
                }).AsQueryable();
        }

        /// <summary>
        /// Scroll al inicio del view y elimina la imagen del imageviewer
        /// </summary>
        internal void ResetView()
        {
            VisualTreeHelperEx.FindDescendantByType<ScrollViewer>(lbxPdfImages)?.ScrollToTop();
        }

        /// <summary>
        /// Actualiza el grid público.
        /// </summary>
        /// <param name="removeCache">Indica si remover el cache, por defecto False.</param>
        private void UpdatePublicPage(bool removeCache = false)
        {
            if (indexadorGridPublicPager.PagedSource != null)
            {
                IQueryable<CarpetaModel> query = UpdatePublicosQuery();
                _lotesPublicos = new ObservableCollection<CarpetaModel>(query.ToList());
                int itemsCount = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.t_lote.id_proyecto == GlobalClass.id_proyecto && x.estado == "D" && x.idusr_asignado == null).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / indexadorGridPublicPager.PageSize);
                indexadorGridPublicPager.PageCount = pageCount;
                if (_lotesPublicos?.Count != 0)
                {
                    indexadorGridPublicPager.LoadDynamicItems(_publicosStartIndex, _lotesPublicos);
                    if (removeCache)
                        indexadorGridPublicPager.PagedSource?.ResetCache();
                }
                else if (_lotesPublicos.Count == 0)
                {
                    indexadorGridPublicPager.PageCount = 0;
                    indexadorGridPublicPager.LoadDynamicItems(_userStartIndex, _lotesPublicos);
                    if (indexadorGridPublicPager.PageIndex != -1)
                        indexadorGridPublicPager.PagedSource?.ResetCacheForPage(indexadorGridPublicPager.PageIndex);
                }
            }
        }

        private void BtnConfigMicro_Click(object sender, RoutedEventArgs e)
        {
            //muestra con configuración del micrófono
            _microWindow.Show();
        }

        private void _microWindow_Closed(object sender, EventArgs e) => UpdateMicroSelection();

        /// <summary>
        /// Actualiza la selección del mcrófono.
        /// </summary>
        private void UpdateMicroSelection()
        {
            _microWindow.Closed -= _microWindow_Closed;
            _microWindow = null;
            _microWindow = new ConfiguracionMicro();
            _microWindow.Closed += _microWindow_Closed;
            if (File.Exists(ConfigurationFile))
            {
                StaticDeepSpeech.Initialize(); //reinicia el cliente DeepSpeech.
                string selectedMicrophone = JsonConvert.DeserializeObject<MicroConfig>(File.ReadAllText(ConfigurationFile, Encoding.UTF8)).SelectedMicrophone;
                StaticDeepSpeech.SetMicrophone(selectedMicrophone); //Actualiza la nueva selección en el cliente DeepSpeech.
            }
            else
            {
                // MessageBox.Show("Selecciona un dispositivo de entrada.");
            }
        }

        /// <summary>
        /// Carga la configuración del micrófono e inicia la instancia DeepSpeech con el micrófono seleccionado.
        /// </summary>
        private void LoadDeepSpeechConfig()
        {
            if (!File.Exists(ConfigurationFile))
            {
                _microWindow.Show();
            }
            else
            {
                StaticDeepSpeech.Initialize();
                MicroConfig microConfig = JsonConvert.DeserializeObject<MicroConfig>(File.ReadAllText(ConfigurationFile, Encoding.UTF8));
                string selectedMicrophone = microConfig.SelectedMicrophone;
                var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Capture, DeviceState.Active);
                int selectedIndex = devices.ToList().FindIndex(x => selectedMicrophone == x.FriendlyName);
                if (!string.IsNullOrEmpty(selectedMicrophone) && selectedIndex != -1)
                {
                    StaticDeepSpeech.SetMicrophone(selectedMicrophone);
                }
                else
                {
                    if (microConfig.Show)
                    {
                        _microWindow.Show();
                    }
                    //MessageBox.Show("No se puede seleccionar la entrada de audio anterior, seleccione una nueva");
                }
            }
        }

        private void BtnIndexar_Click(object sender, RoutedEventArgs e)
        {
            btnIndexar.IsEnabled = false;
            Indexar();
        }

        private void Indexar()
        {
            timer.Start();
        }

        private bool PagIndexada(int p)
        {
            bool b = false;
            foreach (var item in PagBloqueadas)
            {
                if (p >= item.inicio && p <= item.Fin)
                {
                    b = true;
                    return b;
                }
            }
            return b;
        }

        public void BloqueaRango(int _i, int _f)
        {
            LockedPages rangoBloqueado = new LockedPages();
            rangoBloqueado.inicio = _i;
            rangoBloqueado.Fin = _f;
            PagBloqueadas.Add(rangoBloqueado);
        }

        public bool validaRango()
        {
            bool val = true;
            int _i = GlobalClass.selPagInicial;
            int _f = GlobalClass.selPagFinal;
            for (int c = _i; c <= _f; c++)
            {
                if (PagIndexada(c)) val = false;
            }
            return val;
        }

        public void LimpiaComboImg()
        {
            cbpositionNewImg.Items.Clear();
            cbpositionDelImg.Items.Clear();
            cbpositionRotateImg.Items.Clear();
            cbpositionAddReplaceImg.Items.Clear();
        }

        public void AdicionaComboImg(int NumPagina)
        {
            cbpositionNewImg.Items.Add(NumPagina);
            cbpositionDelImg.Items.Add(NumPagina);
            cbpositionRotateImg.Items.Add(NumPagina);
            cbpositionAddReplaceImg.Items.Add(NumPagina);
        }

        public async void OcultaImgIndexadas()
        {   //Hace una copia de Pages con las páginas disponibles, es decir que no tiene en cuenta las páginas que ya están indexadas
            _pagesTEMP.Clear();
            LimpiaComboImg();
            lbxPdfImages.ItemsSource = null;
            bool actualizarvista = false;
            foreach (var p in _pages)
            {
                int pidx = 0;
                pidx = GlobalClass.GetNumber(p.Index);    //int.TryParse(p.Index, out pidx);
                if (!PagIndexada(pidx))
                {
                    var exist = _pagesTEMP.FirstOrDefault(x => x.Index == p.Index);
                    if (exist == null)
                    {
                        _pagesTEMP.Add(p);   //Si la página ya está indexada, no la tiene en cuenta
                        AdicionaComboImg(pidx);
                        if (!actualizarvista)
                        {
                            UpdateSelectedImage(pidx);
                            actualizarvista = true;
                        }
                    }

                }
                //else {} Páginas ya indexadas
            }

            List<t_documento> documents = EntitiesRepository.Entities.t_documento.AsNoTracking()
                            .Where(x => x.id_carpeta == _selectedItem.IdCarptera && x.requiere_seleccion)
                            .ToList();
            
            int pageCount = 0;

            foreach(var d in documents)
            {
                if (d.pag_fin != null && d.pag_fin != null)
                {
                    pageCount = pageCount + GlobalClass.GetNumber(d.pag_fin.ToString()) - GlobalClass.GetNumber(d.pag_ini.ToString()) + 1;
                }
            }
            
            //lbxPdfImages.ItemsSource = null;
            if (pdfviewer.LoadedDocument.Pages.Count == pageCount)   //Si es cero quiere decir que y todas las Imágenes están indexadas y debe cambiar estado de Carpeta = 1
            {
                //Valida si la carpeta tiene al menos un documento para que pueda cambiar de estado
                int vidCarpeta = _selectedItem.IdCarptera;
                int numDocs = EntitiesRepository.Entities.t_documento.AsNoTracking().Where(x => x.id_carpeta == vidCarpeta).Count();
                if (numDocs > 0)
                {
                    Console.WriteLine("Update");
                    var Sql = "update t_carpeta set id_usuario = '" + GlobalClass.id_usuario + "',fecha_indexa=getdate(),estado = 'I',idusr_control = null where id = " + vidCarpeta;
                    await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync(Sql);

                    EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Historico estado
                    {
                        fase = "I",
                        id_carpeta = vidCarpeta,
                        id_usuario = GlobalClass.id_usuario,
                        fecha_estado = DateTime.Now,
                        rechazado = 0
                    });
                    await EntitiesRepository.Entities.SaveChangesAsync();
                    btnDatosLote.IsEnabled = false;
                    btnIndexar.IsEnabled = false;
                    UpdateSelectedImage();
                    PagBloqueadas.Clear();
                    indexDocumentos.Visibility = Visibility.Hidden;
                    _pages.Clear();

                    //pdfviewer falta dispose
                    //imageViewer.Image = null;
                    nestedTabController.SelectedIndex = 0;
                    UpdateWithReset(); // limpia el cache de la página actual, luego navega a la misma página para cargar los datos nuevos y no del cache
                }
            }
            else if (pdfviewer.LoadedDocument.Pages.Count < pageCount)
            {
                MessageBox.Show("Existe error en lor rangos, por favor ingrese a editar carpeta y haga la corrección con la opción 'EDITAR RANGOS'");
            }else
            {
                cbpositionNewImg.Items.Add(_pages.Count + 1);
                if(_pagesTEMP.Count > 0)
                {
                    lbxPdfImages.ItemsSource = _pagesTEMP.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
                    ordenScroll();
                }//_pagesTEMP.OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
            }
        }

        private int CountPages(int? start, int? end)
        {
            return end - start == 0 ? 1 : (int)end - (int)start + 1;
        }

        private void ChkLibOrden_Click(object sender, RoutedEventArgs e)
        {
            ordenScroll();
        }
        

        public void ordenScroll()
        {
            this.ChkLibOrden.Content = "Incio";
            if (_pagesTEMP?.Count > 0)
            {

                if ((bool)this.ChkLibOrden.IsChecked) lbxPdfImages.ScrollIntoView(_pagesTEMP.FirstOrDefaultDynamic());
                else
                {
                    lbxPdfImages.ScrollIntoView(_pagesTEMP.LastOrDefaultDynamic());
                    this.ChkLibOrden.Content = "Final";
                }
            }
            else
            {
                if ((bool)this.ChkLibOrden.IsChecked) lbxPdfImages.ScrollIntoView(_pages.FirstOrDefaultDynamic());
                else
                {
                    lbxPdfImages.ScrollIntoView(_pages.LastOrDefaultDynamic());
                    this.ChkLibOrden.Content = "Final";
                }
            }

        }

        public void PaginasPendientes()
        {
            if (_selectedItem != null)
            {
                PagBloqueadas.Clear();
                //Consulta en la base de datos las páginas ya indexadas
                var indPagesFolder = EntitiesRepository.Entities.t_documento.AsNoTracking().Where(x => x.id_carpeta == _selectedItem.IdCarptera).ToList();

                indexDocumentos.SetIndexados(indPagesFolder.Count, true);

                int _i = 0, _f = 0;
                foreach (var item in indPagesFolder)
                {
                    _i = GlobalClass.GetNumber(item.pag_ini.ToString());  //int.TryParse(item.pag_ini.ToString(), out _i);
                    _f = GlobalClass.GetNumber(item.pag_fin.ToString());  //int.TryParse(item.pag_fin.ToString(), out _f);
                    BloqueaRango(_i, _f);
                }
                OcultaImgIndexadas();
            }
        }

        public void Close()
        {
            if (_datosBasicosLote != null && _datosBasicosLote.IsVisible)
            {
                _datosBasicosLote.Close();
            }
        }

        private async void BtnLiberarLote_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"¿Desea liberar el elemento seleccionado?", "Se requiere confirmación", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (_selectedItemsUser != null && _selectedItemsUser.Count != 0)
                {
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        foreach (CarpetaModel selectedItem in _selectedItemsUser)
                        {
                            context.t_carpeta.FirstOrDefault(x => x.id == selectedItem.IdCarptera).idusr_asignado = null;
                        }
                        await context.SaveChangesAsync();
                        UpdateWithReset();
                        _pages.Clear();
                        btnTomarLote.IsEnabled = false;
                        btnDatosLote.IsEnabled = false;
                    }
                    
                }
                else if (_selectedItem != null)
                {
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        context.t_carpeta.FirstOrDefault(x => x.id == _selectedItem.IdCarptera).idusr_asignado = null;
                        await context.SaveChangesAsync();
                        UpdateWithReset();
                        _pages.Clear();
                        btnTomarLote.IsEnabled = false;
                        btnDatosLote.IsEnabled = false;
                    }
                }
            }
            else
            {
                btnTomarLote.IsEnabled = false;
                btnDatosLote.IsEnabled = false;
            }
        }

        /// <summary>
        /// Actualiza las páginas públicos y mis lotes al reiniciarl el cache.
        /// </summary>
        internal void UpdateWithReset()
        {
            btnActualizar.IsEnabled = false;
            //Se reinicia el cache y se navega a la pagian por lo que fuerza la recarga de los elementos.
            try
            {
                if (indexadorGridPublicPager.PagedSource != null)
                {
                    indexadorGridPublicPager.PagedSource?.ResetCache();
                    if (indexadorGridPublicPager.PageIndex != -1)
                        indexadorGridPublicPager.PagedSource?.ResetCacheForPage(indexadorGridPublicPager.PageIndex);
                    indexadorGridPublicPager.PagedSource?.MoveToPage(indexadorGridPublicPager.PageIndex);
                }
                else
                {
                    UpdatePublicPage(true);
                }

                if (indexadorGridUserPager.PagedSource != null)
                {
                    indexadorGridUserPager.PagedSource?.ResetCache();
                    if (indexadorGridPublicPager.PageIndex != -1)
                        indexadorGridUserPager.PagedSource?.ResetCacheForPage(indexadorGridUserPager.PageIndex);
                    indexadorGridUserPager.PagedSource?.MoveToPage(indexadorGridUserPager.PageIndex);
                }
                else
                {
                    UpdateUserPage(true);
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
            btnActualizar.IsEnabled = true;
        }

        private async void BtnTomarLote_Click(object sender, RoutedEventArgs e)
        {
            await SetLoteCurrentUser();
        }

        /// <summary>
        /// Asigna el lote al usuario actual.
        /// </summary>
        /// <returns>Task</returns>
        private async Task SetLoteCurrentUser()
        {
            if (_selectedItem != null)
            {
                btnIndexar.IsEnabled = false;
                if (_selectedItemsPublicos != null && _selectedItemsPublicos.Count != 0)
                {
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        foreach (CarpetaModel selectedItem in _selectedItemsPublicos)
                        {
                            if ((await context.t_carpeta.FindAsync(selectedItem.IdCarptera)).idusr_asignado == null)
                            {
                                context.t_carpeta.Where(x => x.id == selectedItem.IdCarptera).UpdateFromQuery(x => new t_carpeta
                                { idusr_asignado = GlobalClass.id_usuario });
                            }
                            else
                            {
                                MessageBox.Show("La carpeta ya se ha asignado.");
                                UpdateWithReset();
                            }
                        }
                        await context.SaveChangesAsync();

                        UpdateWithReset();
                        _pages.Clear();
                        _currentItem = null;
                        btnTomarLote.IsEnabled = false;
                        btnDatosLote.IsEnabled = false;
                    }
                }
                else if (_selectedItem != null)
                {
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        if ((await context.t_carpeta.FindAsync(_selectedItem.IdCarptera)).idusr_asignado == null)
                        {
                            context.t_carpeta.Where(x => x.id == _selectedItem.IdCarptera).UpdateFromQuery(x => new t_carpeta
                            { idusr_asignado = GlobalClass.id_usuario });

                            await context.SaveChangesAsync();
                            UpdateWithReset();
                            _pages.Clear();
                            _currentItem = null;
                            btnTomarLote.IsEnabled = false;
                            btnDatosLote.IsEnabled = false;
                        }
                        else
                        {
                            MessageBox.Show("La carpeta ya se ha asignado.");
                            UpdateWithReset();
                        }
                    }
                }
            }
        }

        private void BtnDatosLote_Click(object sender, RoutedEventArgs e)
        {
            _userSelectedIndex = indexadorGridUser.SelectedIndex;
            _publicosSelectedIndex = indexadorGridPublicos.SelectedIndex;

            if (GlobalClass.Carpeta == null) return;
            _datosBasicosLote = new DatosBasicosLoteWindow();
            _datosBasicosLote.SetSelectedCarpeta(this, _selectedItem); //se envían los datos básicos si ya existen.
            _datosBasicosLote.Topmost = true;
            _datosBasicosLote.Show();
            _datosBasicosLote.Closed += _datosBasicosLote_Closed;
            Dispatcher.BeginInvoke((Action)(() => nestedTabController.SelectedIndex = 1));
            btnDatosLote.IsEnabled = false;
        }

        /// <summary>
        /// Guarda el pdf y verifica las páginas buscando ediciones.
        /// </summary>
        public void RemoveFromPdf()
        {
            var selectedItems = lbxPdfImages.SelectedItems.Cast<PDFPages>().ToList(); //copia que evita el enumerator se cierre si se remueve un elemento
            if (selectedItems.Select(x => x.Edited).Contains(true))
            {
                var edited = selectedItems.Select(x => x.Edited).Contains(true); //verificamos si la lista se ha editado.
                SaveNewPdf();
            }
            //else if ()
            //{
            //}
        }

        private async Task RemoverPaginaPdfAsync(int numpagina, string observacion)
        {
            //try
            //{
            _ruta = GlobalClass.ruta_proyecto + $@"/{_selectedItem.t_lote.nom_lote}/{_selectedItem.nro_caja}/{_selectedItem.nro_expediente}/{_selectedItem.nro_expediente}.pdf";
            if (!File.Exists(_ruta)) _ruta = GlobalClass.ruta_proyecto + $@"/{_selectedItem.t_lote.nom_lote}/{_selectedItem.nro_caja}/{_selectedItem.nro_expediente}.pdf";
            if (!File.Exists(_ruta))
            {
                MessageBox.Show("No se encontró el archivo!");
                return;
            }

            string folderCopia = GlobalClass.ruta_proyecto + $@"/Modificados/{_selectedItem.t_lote.nom_lote}/{_selectedItem.nro_caja}/";
            string pdfCopia = folderCopia + $@"{_selectedItem.nro_expediente}.pdf";

            //Realiza copia del archivo Original
            Directory.CreateDirectory(folderCopia);  //crear archivo si no existe
            if (File.Exists(pdfCopia)) //si existe al archivo hace copia de la uptima copia
            {
                string oldFileName = pdfCopia.TrimEnd(".pdf".ToCharArray()) + $"old{Guid.NewGuid()}.pdf";
                File.Move(pdfCopia, oldFileName);
                File.Delete(pdfCopia);
            }
            File.Move(_ruta, pdfCopia); //crea copia del archivo que se está editando

            PdfLoadedDocument document = new PdfLoadedDocument(pdfCopia);

            //Remove the second page
            document.Pages.RemoveAt(numpagina - 1);

            //Save the PDF document
            document.Save(_ruta);

            //Close the instance of PdfLoadedDocument
            document.Close(true);

            //ajusta las los números de las siguientes imágenes
            List<t_documento> res = EntitiesRepository.Entities.t_documento.Where(r => r.id_carpeta == _selectedItem.IdCarptera && r.pag_ini > numpagina).ToList();
            // update
            foreach (var r in res)
            {
                r.pag_ini = r.pag_ini - 1;
                r.pag_fin = r.pag_fin - 1;
            }
            // save
            EntitiesRepository.Entities.SaveChanges();

            //Guarda registro de eliminación
            EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Historico Carpeta
            {
                id_carpeta = _selectedItem.IdCarptera,
                id_usuario = GlobalClass.id_usuario,
                fase = "D",
                fecha_estado = DateTime.Now,
                observacion = $@"Eliminación pág {numpagina}: {observacion}",
                modificacion_pdf = true,
            });
            await EntitiesRepository.Entities.SaveChangesAsync();
            ReloadPdf(_ruta);
            //}
            //catch (IOException)
            //{
            //    MessageBox.Show("El pdf está en uso, es necesario cerrarlo para eliminar páginas.");
            //}
        }

        

        /// <summary>
        /// Guarda el nuevo pdf.
        /// </summary>
        private async void SaveNewPdf()
        {
            ArchivoMeta metaFile = PDFHelper.CopiaPdf(_selectedItem, true);
            _ruta = metaFile.RutaFuente;
            string pdfCopia = metaFile.RutaCopia;
            if (string.IsNullOrEmpty(pdfCopia))
            {
                MessageBox.Show(metaFile.ErrorMsj);
                return;
            }

            PdfDocument doc = new PdfDocument();
            PdfLoadedDocument oldPdfDoc = new PdfLoadedDocument(pdfCopia);
            for (int index = 0; index < _pages.Count; index++)
            {
                PDFPages pdfPage = _pages[index];
                PdfPage page = doc.Pages.Add();

                //Create PDF graphics for the page

                PdfGraphics graphics = page.Graphics;

                //Load the image from the disk
                SizeF pageSize = page.GetClientSize();

                //Setting image bounds
                RectangleF imageBounds = new RectangleF(0, 0, pageSize.Width, pageSize.Height);

                if (pdfPage.Edited)
                {
                    PdfBitmap image = new PdfBitmap(pdfPage.ImageData.StreamFromBitmapSource());
                    //Draw the image
                    graphics.DrawImage(image, imageBounds);
                }
                else
                {
                    int indexOriginal = GlobalClass.GetNumber(pdfPage.IndexOld) - 1;
                    var image = oldPdfDoc.Pages[indexOriginal].ExtractImages().FirstOrDefault();
                    if (image != null)
                    {
                        graphics.DrawImage(new PdfBitmap(image.ToImageSource().StreamFromBitmapSource()), imageBounds);
                        image.Dispose();
                    }
                }
            }
            //Save the document

            doc.Save(_ruta);

            //Close the document

            doc.Close(true);
            ReloadPdf(_ruta);
        }

        /// <summary>
        /// Actualiza la imagen seleccionada.
        /// </summary>
        /// <param name="selectedIndex">Index de la imgagen seleccionada. Por defecto -1</param>
        private void UpdateSelectedImage(int selectedIndex = -1)
        {
            bool actualizaTodo = true;
            if (selectedIndex > -1)
                actualizaTodo = false;
            if (_pages.Count > 0)
            {
                Dispatcher.Invoke(() =>
                {
                    if (selectedIndex == -1)
                    {
                        SetImageToViewer(0);
                    }
                    else
                    {
                        SetImageToViewer(selectedIndex - 1);
                    }
                });
                if (actualizaTodo || (lbxPdfImages.Items.Count == 0 && _pages.Count > 0))
                    lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
                pdfCargado = true;
                txtNomPdf.Text = _selectedItem.NoExpediente;
            }
        }

        //Carga la imágen en el editor de imágen
        private void SetImageToViewer(int selectedIndex)
        {
            if (selectedIndex > _pages.Count()) selectedIndex = _pages.Count() - 1;
            _selectedImageIndex = selectedIndex + 1;
            if (_pdfViewerLoaded)
            {
                pdfviewer.CurrentPage = _selectedImageIndex;
            }
            //pdfviewer falta dispose
            //imageViewer.Image?.Dispose();
            if (_pages[selectedIndex].Edited)
            {
                //PdfBitmap image = new PdfBitmap(_pages[selectedIndex].ImageData.ToStream());
                //pdfviewer falta adaptar img editadas
                /// pdfviewer.CurrentPage = selectedIndex;
                // imageViewer.Image = _pages[selectedIndex].Source.StreamFromBitmapSource();
            }
            else
            {
                int indexOriginal = GlobalClass.GetNumber(_pages[selectedIndex].IndexOld) - 1;
                // YA NO SE NECESITA YA QUE NO HAY EDITOR DE IMÁGEN, FUÉ REEMPLAZADO POR EL PDFVIEWER
                //var image = _loadedDocument.Pages[indexOriginal].ExtractImages().FirstOrDefault();
                ////pdfviewer falta adaptar img editadas
                ////imageViewer.Image?.Dispose();
                ////imageViewer.Image = image.ToImageSource().StreamFromBitmapSource();
                //image.Dispose();
            }
        }

        private void lbxPdfImages_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            int numSeleccionados = lbxPdfImages.SelectedItems.Count;
            if (numSeleccionados == 0) return;
            int numPagIni = 9999999, numPagFin = 0, numPagSel = 0;
            int ttalImgSeleccion = lbxPdfImages.SelectedItems.Count;
            for (int p = 0; p < ttalImgSeleccion; p++)
            {
                PDFPages pp = (PDFPages)lbxPdfImages.SelectedItems[p];
                int tmpNum = GlobalClass.GetNumber(pp.Index);
                if (tmpNum < numPagIni) numPagIni = tmpNum;
                if (tmpNum > numPagFin) numPagFin = tmpNum;
                if (p == (ttalImgSeleccion-1)) numPagSel = tmpNum; //OJO: Esto aveces cambia con la versión del Listview
            }
            //Guarda en la variable global la página de inicio y Fin Seleccionadas
            GlobalClass.selPagInicial = numPagIni;
            GlobalClass.selPagFinal = numPagFin;

            //Actualiza la página en la pestaña imágen
            UpdateSelectedImage(numPagSel);//Muestra la última imágen seleccionada
            if (lbxPdfImages.SelectedIndex != -1)
            {
                nestedTabController.SelectedIndex = 1;
            }
        }

        private void IndexadorView_Loaded(object sender, RoutedEventArgs e)
        {
            nestedTabController.SelectedIndex = 0;
            if (GlobalClass.loc_admin == 1 || GlobalClass.loc_index == 1) FillGrids();
            else MessageBox.Show("No tiene acceso a indexación, por favor contacte al administrador del sistema");
            LastItemSelectedUser = -2;
            LastItemSelectedPublic = -2;
        }

        /// <summary>
        /// Registra los cambios de index en los grids y carga los lotes.
        /// </summary>
        private void FillGrids()
        {
            indexadorGridPublicos.SelectionChanged += IndexadorPublicosSelectionChanged;
            indexadorGridUser.SelectionChanged += IndexadorGridUser_SelectionChanged;
            if (_lotesPublicos != null)
            {
                _lotesPublicos = new ObservableCollection<CarpetaModel>(_lotesPublicos.Where(x => !x.IdUsuario.HasValue).ToList());
            }
            if (_misLotes != null)
            {
                _misLotes = new ObservableCollection<CarpetaModel>(_misLotes.Where(x => GlobalClass.id_usuario == x.IdUsuario).ToList());
            }
            RefreshPublicos(_publicosStartIndex);
        }

        private async void IndexadorGridUser_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {

            try
            {
                
                btnEliminar.IsEnabled = false;
                _selectedItemsPublicos = null;
                _pagesTEMP.Clear();
                if (_datosBasicosLote!= null)
                {
                    _datosBasicosLote.Closed -= _datosBasicosLote_Closed;
                    _datosBasicosLote?.Close();
                    _enableReloadPdf = true;
                }
                if (indexadorGridUser.SelectedItems.Count == 1)
                {
                    var selectedItem = (CarpetaModel)indexadorGridUser.SelectedItem;
                    if (selectedItem != null)
                    {
                        if (LastItemSelectedUser == selectedItem.IdCarptera) return;
                        else LastItemSelectedUser = selectedItem.IdCarptera;
                    }

                    //BusyIndicator.IsBusy = true;
                    _selectedItemsUser = null;
                    
                    if (selectedItem != null)
                    {
                        GlobalClass.UserSelectedIndex = indexadorGridUser.SelectedIndex;
                        btnDeleteImage.Visibility = Visibility.Hidden;
                        btnAddImage.Visibility = Visibility.Hidden;
                        btnDeleteImage.Visibility = Visibility.Hidden;
                        btnRotateImage.Visibility = Visibility.Hidden;
                        btnAddReplacePDF.Visibility = Visibility.Hidden;
                        btnOpenWorkingFolder.Visibility = Visibility.Hidden;
                        txtNomPdf.Text = "";
                        SelectUserIndex(selectedItem);
                        //btnEliminar.IsEnabled = true;
                    }
                }
                else
                {
                    btnIndexar.IsEnabled = false;
                    btnDatosLote.IsEnabled = false;
                    _selectedItemsUser = indexadorGridUser.SelectedItems;
                    //btnEliminar.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                BusyIndicator.IsBusy = false;
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Actualiza el index del grid de los lotes del usuario.
        /// </summary>
        /// <returns></returns>
        private void SelectUserIndex(CarpetaModel selectedItem)
        {
            if (selectedItem != null)
            {
                btnDatosLote.IsEnabled = true;
                //pdfviewer
                //imageViewer.ToolbarSettings.IsToolbarVisiblity = false;
                _selectedItem = selectedItem;

                IndexSelectedItem(selectedItem, _loadImagesToken);
            }
            else
            {
                btnDatosLote.IsEnabled = false;
            }
        }

        private async void IndexadorPublicosSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                BusyIndicator.IsBusy = true;
                _selectedItemsUser = null;
                if (_datosBasicosLote != null)
                {
                    _datosBasicosLote.Closed -= _datosBasicosLote_Closed;
                    _datosBasicosLote?.Close();
                    _enableReloadPdf = true;
                }

                if (indexadorGridPublicos.SelectedItems.Count == 1)
                {
                    if (LastItemSelectedPublic== indexadorGridPublicos.SelectedIndex) return;
                    else LastItemSelectedPublic = indexadorGridPublicos.SelectedIndex;
                    _selectedItemsPublicos = null;

                    btnDeleteImage.Visibility = Visibility.Hidden;
                    btnAddImage.Visibility = Visibility.Hidden;
                    btnRotateImage.Visibility = Visibility.Hidden;
                    btnAddReplacePDF.Visibility = Visibility.Hidden;
                    btnOpenWorkingFolder.Visibility = Visibility.Hidden;
                    var selectedItem = (CarpetaModel)indexadorGridPublicos.SelectedItem;
                    if (selectedItem != null)
                    {
                        SelectPublicIndex(selectedItem);
                        if (selectedItem.CarpetaEstado.Count != 0)
                        {
                            lblAdminMsg.Text = selectedItem.CarpetaEstado.FirstOrDefault().observacion;
                        }
                        else
                        {
                            lblAdminMsg.Text = "";
                        }
                        btnEliminar.IsEnabled = true;
                    }
                }
                else
                {
                    BusyIndicator.IsBusy = false;
                    btnIndexar.IsEnabled = false;
                    btnDatosLote.IsEnabled = false;
                    _selectedItemsPublicos = indexadorGridPublicos.SelectedItems;
                    btnEliminar.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Actualiza el index del grid de los lotes públicos.
        /// </summary>
        /// <returns></returns>
        private void SelectPublicIndex(CarpetaModel selectedItem)
        {
            if (selectedItem != null)
            {
                btnDatosLote.IsEnabled = true;
                _selectedItem = selectedItem;
                if (_loadImagesToken != null && _loadImagesToken.CanBeCanceled)
                {
                    tokenSource.Cancel();
                }
                tokenSource = new CancellationTokenSource();
                _loadImagesToken = tokenSource.Token;
                IndexSelectedItem(selectedItem, _loadImagesToken);
            }
            else
            {
                btnDatosLote.IsEnabled = false;
            }
        }

        /// <summary>
        /// Seleccionar pdf y cargarlo al listView.
        /// </summary>
        /// <returns></returns>
        private async void IndexSelectedItem(CarpetaModel carpetaModel, CancellationToken _loadImagesToken)
        {
            try
            {
                IsLoadingImages = true;
                if ((_currentItem != carpetaModel && carpetaModel != null)
                    || (_misLotes?.Count == 1)
                    || (_lotesPublicos?.Count == 1))
                {
                    if (_loadImagesToken != null && _loadImagesToken.CanBeCanceled)
                    {
                        tokenSource.Cancel();
                    }
                    tokenSource = new CancellationTokenSource();
                    _loadImagesToken = tokenSource.Token;

                    _currentItem = carpetaModel;

                    GlobalClass.Carpeta = _selectedItem;
                    _ruta = GlobalClass.ruta_proyecto + $@"/{_selectedItem.t_lote.nom_lote}/{_selectedItem.nro_caja}/{_selectedItem.nro_expediente}/{_selectedItem.nro_expediente}.pdf";
                    if (!File.Exists(_ruta))
                    {
                        _ruta = GlobalClass.ruta_proyecto + $@"/{_selectedItem.t_lote.nom_lote}/{_selectedItem.nro_caja}/{_selectedItem.nro_expediente}.pdf";
                    }

                    _selectedItem.PdfDir = _ruta;
                    if (File.Exists(_ruta))
                    {
                        BusyIndicator.IsBusy = true;

                        //ReloadPdf(_ruta);
                        //verifica si el index es públicos
                        if (lotesTab.SelectedIndex == 1)
                        {
                            //bloquea indexado
                            btnIndexar.IsEnabled = false;
                            btnDatosLote.IsEnabled = false;
                        }
                        else
                        {
                            //habilita indexado
                            btnDatosLote.IsEnabled = true;
                            btnIndexar.IsEnabled = !string.IsNullOrEmpty(_selectedItem.nom_expediente); //se veriica si ta tiene los datos básicos para habilitar el indexado
                        }
                        //verificar mensaje admin
                        if (_selectedItem?.CarpetaEstado?.Count != 0)
                        {
                            var Observacion = _selectedItem?.CarpetaEstado.Where(e => e.fase == "D" && e.modificacion_pdf != true).OrderByDescending(x => x.fecha_estado).FirstOrDefault();
                            if (Observacion != null)
                            {
                                MessageBox.Show(Observacion.observacion,
                                    "Mensaje del administrador",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        if (indexDocumentos.Visibility == Visibility.Visible)
                        {
                            lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
                        }
                        int indexDoc = 1;
                        if (_enableReloadPdf)
                        {
                            pagesClear();
                            
                            try
                            {
                                if (pdfviewer.LoadedDocument != null) //Si hay un documento cargado lo cierra, inicializa la variable con null
                                {
                                    pdfviewer.LoadedDocument.Close(true);
                                }
                                if (pdfviewer != null)
                                {
                                    pdfviewer.Unload(true);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            ReloadPdf(_ruta);
                        }

                        try
                        {
                            if (pdfviewer.LoadedDocument.Pages.Count > 0)
                            {
                                pdfCargado = false; //Si hay paginas para cargar se cambia el estado
                            }
                        }
                        catch (NullReferenceException)
                        {

                        }
                        
                       

                        try
                        {
                            BusyIndicator.IsBusy = true;
                            if (_enableReloadPdf)
                            {
                                foreach (PdfPageBase page in pdfviewer.LoadedDocument.Pages) //Carga en _pages cada una de las páginas
                                {
                                    if (!_loadImagesToken.IsCancellationRequested)
                                    {
                                        if (pdfviewer.IsEnabled)
                                        {
                                            var exist = _pages.FirstOrDefault(x => x.Index == indexDoc.ToString());
                                            if (exist == null)
                                            {
                                                BitmapSource image = await Task.Run(() => GetImage(indexDoc), _loadImagesToken);
                                                if (image != null)
                                                {
                                                    _pages.Insert(indexDoc - 1, new PDFPages
                                                    {
                                                        Source = image,
                                                        Index = indexDoc.ToString(),
                                                        IndexOld = indexDoc.ToString()
                                                    });
                                                    indexDoc++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                            BusyIndicator.IsBusy = false;
                            if (!_loadImagesToken.IsCancellationRequested)
                            {
                                int defecto = 1;
                                if (_selectedItem.PagDefecto != null) defecto = GlobalClass.GetNumber(_selectedItem.PagDefecto.ToString());
                                UpdateSelectedImage(defecto);
                            }
                            else
                            {
                                pdfviewer.Unload(true);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (_selectedItem.IdUsuario == GlobalClass.id_usuario)
                        {
                            btnLiberarLote.IsEnabled = true;
                            btnTomarLote.IsEnabled = false;
                        }
                        else
                        {
                            btnTomarLote.IsEnabled = true;
                            btnLiberarLote.IsEnabled = false;
                        }
                        indexDocumentos.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        if (_loadImagesToken != null && _loadImagesToken.CanBeCanceled)
                        {
                            tokenSource.Cancel();
                        }
                        btnLiberarLote.IsEnabled = false;
                        btnDatosLote.IsEnabled = false;
                        _pages.Clear();
                        BusyIndicator.IsBusy = false;
                        MessageBox.Show("No se ha encontrado el archivo: \n " + _ruta);
                    }
                }
                IsLoadingImages = false;
            }
            catch (PdfException)
            {
                BusyIndicator.IsBusy = false;
                MessageBox.Show($"PDF corrupto, error al cargar: {_ruta}");
                btnDatosLote.IsEnabled = false;
                btnIndexar.IsEnabled = false;
            }
        }

        /// <summary>
        /// Recarga el pdf.
        /// </summary>
        /// <param name="pdfPath">Dirección del archivo pdf.</param>
        private void ReloadPdf(string pdfPath)
        {
            try
            {
                var cachePath = CacheHelper.LoadFromCache(pdfPath);

                pdfviewer.Load(string.IsNullOrWhiteSpace(cachePath) ? pdfPath : cachePath);

                _pdfViewerLoaded = false;
                pdfviewer.ZoomMode = ZoomMode.FitWidth;
            }
            catch (PdfException)
            {
                MessageBox.Show($"PDF corrupto, error al cargar: {pdfPath}");
            }
        }

        private BitmapSource GetImage(int indexDoc)
        {
            try
            {
                BitmapSource bitmapSource = pdfviewer.ExportAsImage(indexDoc - 1, customSize: new SizeF(69f, 114f), false);
                return bitmapSource;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void pagesClear()
        {
            if (_pages.Count != 0)  //Si hay lista de paginas cargadas, inicializa la variable con null
            {
                for (int i = 0; i < _pages.Count; i++)
                {
                    _pages[i].Source = null;
                }
            }
            _pages.Clear();
        }

        private void ToolbarSettings_ToolbarItemSelected(object sender, Syncfusion.UI.Xaml.ImageEditor.ToolbarItemSelectedEventArgs e)
        {
            //test
        }

        /// <summary>
        /// Detecta que se está guardando una edición de imagen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageViewer_ImageSaving(object sender, Syncfusion.UI.Xaml.ImageEditor.ImageSavingEventArgs e)
        {
            e.Cancel = true;
            if (lbxPdfImages.SelectedIndex != -1)
            {
                var bitmapImage2 = new Bitmap(e.Stream);
                //bitmapImage2.Save("edited.bmp");
                if (lbxPdfImages.SelectedIndex == -1) lbxPdfImages.SelectedIndex = 0;
                _pages[lbxPdfImages.SelectedIndex].ImageData = bitmapImage2.ToBitmapSource(); //aplica el cambio de imagen a list
                _pages[lbxPdfImages.SelectedIndex].Edited = true;
                //lbxPdfImages.ItemsSource = null;
                lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            UpdateWithReset();
        }

        private void btnOpenWorkingFolder_Click(object sender, RoutedEventArgs e)
        {
            //return false; //Process.Start("explorer.exe", new FileInfo(_ruta).Directory.FullName);
        }


        private async void btnLiberarTodo_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"¿Desea liberar TODA su asignación?", "Se requiere confirmación", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes) return;

            btnLiberarTodo.IsEnabled = false;
            var liberar = EntitiesRepository.Entities.t_carpeta.Where(x => x.estado == "D" && x.idusr_asignado == GlobalClass.id_usuario && x.t_lote.id_proyecto == GlobalClass.id_proyecto)
                .UpdateFromQuery(x => new t_carpeta { idusr_asignado = null });
            await EntitiesRepository.Entities.SaveChangesAsync();
            UpdateWithReset();
            btnLiberarTodo.IsEnabled = true;
            _pages.Clear();
            _currentItem = null;
        }

        private void LoadingDialogHost_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {

        }

        private void btnVerNotas_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddNota_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"¿Desea eliminar el elemento seleccionado?", "Se requiere confirmación", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (_selectedItemsUser != null && _selectedItemsUser.Count != 0)
                {
                    var docsUsuario = _selectedItemsUser;
                    await EliminarDocumentos(docsUsuario);
                }
                else if (_selectedItemsPublicos != null && _selectedItemsPublicos.Count != 0)
                {
                    var publicos = _selectedItemsPublicos;
                    await EliminarDocumentos(publicos);
                }
                else if (_selectedItem != null)
                {
                    var idCarpeta = new SqlParameter("idCarpeta", _selectedItem.IdCarptera);
                    var idUsr = new SqlParameter("idUsr", GlobalClass.id_usuario);
                    await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync("exec sp_BorrarCarpeta @idCarpeta, @idUsr", idCarpeta, idUsr);
                    UpdateWithReset();
                    _pages.Clear();
                    btnTomarLote.IsEnabled = false;
                    btnDatosLote.IsEnabled = false;
                    btnEliminar.IsEnabled = false;
                }
            }
            else
            {
                btnTomarLote.IsEnabled = false;
                btnDatosLote.IsEnabled = false;
                btnEliminar.IsEnabled = false;
            }
        }

        private async Task EliminarDocumentos(ObservableCollection<object> documentos)
        {
            foreach (CarpetaModel selectedItem in documentos)
            {
                var idCarpeta = new SqlParameter("idCarpeta", selectedItem.IdCarptera);
                var idUsr = new SqlParameter("idUsr", GlobalClass.id_usuario);
                await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync("exec sp_BorrarCarpeta @idCarpeta, @idUsr", idCarpeta, idUsr);
            }


            UpdateWithReset();
            _pages.Clear();
            btnTomarLote.IsEnabled = false;
            btnDatosLote.IsEnabled = false;
            btnEliminar.IsEnabled = false;
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            btnEditar.IsEnabled = true;
            GlobalClass.FromIndexado = true;
            GlobalClass.ViewController.EnviarCarpetaCalidad(_selectedItem.IdCarptera);
        }

        private void btnBorrarCache_Click(object sender, RoutedEventArgs e)
        {
            CacheHelper.DeleteCache();
        }

        private void btnActualizarCache_Click(object sender, RoutedEventArgs e)
        {
            btnActualizar.IsEnabled = false;
            pbarUpdateCache.Visibility = Visibility.Visible;
            var index = lotesTab.SelectedIndex;
            Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (index == 0)
                        {
                            pbarUpdateCache.Maximum = _misLotes.Count;
                            foreach (var carpeta in _misLotes)
                            {
                                SaveToCache(carpeta);
                                pbarUpdateCache.Value++;
                            }
                        }
                        //else
                        //{
                        //    pbarUpdateCache.Maximum = _lotesPublicos.Count;
                        //    foreach (var carpeta in _lotesPublicos)
                        //    {
                        //        SaveToCache(carpeta);
                        //        pbarUpdateCache.Value++;
                        //    }
                        //}
                    });
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                finally
                {
                    Dispatcher.Invoke(() => pbarUpdateCache.Visibility = Visibility.Collapsed);
                    Dispatcher.Invoke(() => btnActualizar.IsEnabled = true);
                }
            });
        }

        private void SaveToCache(CarpetaModel carptea)
        {
            string ruta = string.Empty;
            ruta = GetRuta(carptea);
            CacheHelper.SaveToCacheAsync(ruta);
        }

        private static string GetRuta(CarpetaModel carptea)
        {
            string ruta = GlobalClass.ruta_proyecto + $@"/{carptea.t_lote.nom_lote}/{carptea.nro_caja}/{carptea.nro_expediente}/{carptea.nro_expediente}.pdf";
            if (!File.Exists(ruta))
            {
                ruta = GlobalClass.ruta_proyecto + $@"/{carptea.t_lote.nom_lote}/{carptea.nro_caja}/{carptea.nro_expediente}.pdf";
            }

            return ruta;
        }
    }
}