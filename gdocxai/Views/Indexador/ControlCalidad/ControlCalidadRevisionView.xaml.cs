using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Helpers;
using Indexai.Models;
using Indexai.Services;
using MaterialDesignThemes.Wpf;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Z.EntityFramework.Plus;

namespace Indexai.Views
{
    /// <summary>
    /// Interaction logic for ControlCalidadRevisionView.xaml
    /// </summary>
    public partial class ControlCalidadRevisionView : UserControl
    {
        private IQueryable<t_documento> _baseFilteredQuery;
        private AdminBeneficiarioWindow _adminBeneficiarioWindow;
        private ControlCalidadAsignadoModel _controlCalidadAsignadoModel;
        private ControlCalidadView _controlCalidadView;
        private int _totalItemCount = -1;
        private List<DocumentReview> _reviewedDocuments;
        private List<ControlCalidadIListItem> _tDocumentResp;
        private ControlCalidadIListItem _selectedDocument;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _loadImagesToken;
        private readonly ObservableCollection<PDFPages> _pages = new ObservableCollection<PDFPages>(new List<PDFPages>());
        private string _pdfName;
        private List<string> _archivadores;
        private int _userStartIndex;
        private DatosBasicosLoteWindow _datosBasicosLote;
        public PdfLoadedDocument _loadedDocument;
        private bool _documenLoaded = false;
        private List<ControlCalidadIListItem> _orderDocuments;
        private bool _firstLoad = true;
        private int LastItemSelected = -2;

        internal void ShowPdfViewer()
        {
            controlCalidadTab.SelectedIndex = 1;
        }

        private string _newPdf;
        private bool _showpdfViewer;
        private IQueryable<t_documento> _baseQueryForReviewCount;
        private CancellationToken _cancelToken;

        public ControlCalidadRevisionView()
        {
            InitializeComponent();
            pdfviewer.Unload(true);
            _documenLoaded = false;
            controlCalidadGrid.SelectionChanged += ControlCalidadGrid_SelectionChanged;
            calidadPager.OnDemandLoading += CalidadPager_OnDemandLoading;
            rowDetailEdit.SetAdmin(true); //Indica que el usuario actual es administrador
            lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index);
            lbxPdfImages.SelectionChanged += LbxPdfImages_SelectionChanged;
            rowDetailEdit.SetControlCalidad(this);
            pdfviewer.DocumentLoaded += Pdfviewer_DocumentLoaded;
            pdfviewer.ToolbarSettings.ShowAnnotationTools = false;
            pdfviewer.ToolbarSettings.ShowFileTools = false;
            pdfviewer.ZoomMode = ZoomMode.FitWidth;
            pdfviewer.WarnBeforeClose = false;
            controlCalidadTab.SelectedIndex = 1;
            Loaded += ControlCalidadRevisionView_Loaded;
        }

        private void ControlCalidadRevisionView_Loaded(object sender, RoutedEventArgs e)
        {
            controlCalidadTab.SelectedIndex = 0;
            controlCalidadTab.SelectedIndex = 1;
            controlCalidadTab.SelectedIndex = 0;
            LastItemSelected = -2;
        }

        internal void MoveFirstPage()
        {
            calidadPager?.MoveToFirstPage();
            calidadPager?.PagedSource?.ResetCache();
        }

        private async void Pdfviewer_DocumentLoaded(object sender, EventArgs args)
        {
            Console.WriteLine("CARGADO: " + pdfviewer.DocumentInfo.FilePath);
            await UpdateOnLoaded();
            _documenLoaded = true;
            CacheHelper.SaveToCacheAsync(pdfviewer, Dispatcher);
        }

        /// <summary>
        /// Cancela la revisión en control de calidad.
        /// </summary>
        internal void Cancel()
        {
            rowDetailEdit.CleanInputs(true);
            _pages.Clear();
        }

        private async void AumentarClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (_tDocumentResp != null && _tDocumentResp.Count != 0)
            {
                if (!Equals(eventArgs.Parameter, true)) return;
                if (!string.IsNullOrWhiteSpace(txtAumentar.Text) && GlobalClass.GetNumber(txtAumentar.Text) != 0)
                {
                    int aumentar = GlobalClass.GetNumber(txtAumentar.Text);
                    await AumentarPags(aumentar);
                }
                else
                {
                    MessageBox.Show("Se requiere un valor mayor a 0.");
                }
            }

            btnDisminuirIndex.IsEnabled = false;
            btnAumentarIndex.IsEnabled = false;
            txtAumentar.Text = string.Empty;
        }



        /// <summary>
        /// Aumenta el índice del documento.
        /// </summary>
        /// <param name="aumentar">Cantidad a aumentar.</param>
        /// <param name="showMessages">Mostrar mensajes.</param>
        /// <param name="fromInsert">Indica si el aumento de está haciendo por insert.</param>
        /// <returns></returns>
        private async Task AumentarPags(int aumentar, bool showMessages = true, bool fromInsert = false)
        {
            List<ControlCalidadIListItem> documentsRange = GetDocumentsRange();
            using (gdocxEntities context = new gdocxEntities())
            {
                int i = fromInsert ? 1 : 0;
                for (; i < documentsRange.Count; i++)
                {
                    ControlCalidadIListItem document = documentsRange[i];
                    var documentAumentar = await context.t_documento.FindAsync(document.Id);
                    documentAumentar.pag_ini += aumentar;
                    documentAumentar.pag_fin += aumentar;
                    await context.SaveChangesAsync();
                }
                rowDetailEdit.Visibility = Visibility.Hidden;
                if (showMessages)
                {
                    UpdateWithReset();
                    MessageBox.Show($"El índice se aumentó en {aumentar} para {documentsRange.Count} documentos.");
                }
            }
        }

        private async void DisminuirClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (_tDocumentResp != null && _tDocumentResp.Count != 0)
            {
                if (!Equals(eventArgs.Parameter, true)) return;
                if (!string.IsNullOrWhiteSpace(txtDisminuir.Text) && GlobalClass.GetNumber(txtDisminuir.Text) != 0)
                {
                    List<ControlCalidadIListItem> documentsRange = GetDocumentsRange();
                    int disminuir = GlobalClass.GetNumber(txtDisminuir.Text);
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        foreach (var document in documentsRange)
                        {
                            var documentAumentar = await context.t_documento.FindAsync(document.Id);
                            documentAumentar.pag_ini -= disminuir;
                            documentAumentar.pag_fin -= disminuir;
                            await context.SaveChangesAsync();
                        }
                    }
                    rowDetailEdit.Visibility = Visibility.Hidden;
                    UpdateWithReset();
                    MessageBox.Show($"El índice se disminuyó en {disminuir} para {documentsRange.Count} documentos.");
                }
                else
                {
                    MessageBox.Show("Se requiere un valor mayor a 0.");
                }
            }

            btnDisminuirIndex.IsEnabled = false;
            btnAumentarIndex.IsEnabled = false;
            txtDisminuir.Text = string.Empty;
        }

        /// <summary>
        /// Obtiene la lista de documentos desde el documento actual.
        /// </summary>
        /// <returns>Lista de documentos.</returns>
        private List<ControlCalidadIListItem> GetDocumentsRange()
        {
            IQueryable<t_documento> query = BuildControlCalidadGridQuery();
            var documentos = QueryToModelList(query.OrderBy(x => x.id)).OrderBy(x => x.PagIni).OrderBy(y => y.PagFin).ToList();
            _orderDocuments = documentos.OrderBy(x => x.PagIni).OrderBy(y => y.PagFin).ToList();
            int currentDocumentIndex = _orderDocuments.FindIndex(x => x.Documento.id == _selectedDocument.Documento.id);
            var documentsRange = _orderDocuments.GetRange(currentDocumentIndex, _orderDocuments.Count - currentDocumentIndex);
            return documentsRange;
        }

        private async void RemoveDocumentResult(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return; //valida si el dialog confirmó eliminar el documento
            if (!string.IsNullOrWhiteSpace(txtMensajeTranscriptores.Text))
            {
                if (_selectedDocument == null)
                {
                    await RemoveAll(_controlCalidadAsignadoModel.id);
                }
                else
                {
                    if (chkEliminarTodo.IsChecked.Value)
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea eliminar TODOS los documentos y su información?", "Importante", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            var carpetaId = _selectedDocument.CarpetaId;
                            await RemoveAll(carpetaId);
                        }
                    }
                    else
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show("¿Seguro que desea eliminar el documento?", "Importante", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {

                            using (gdocxEntities context = new gdocxEntities())
                            {
                                var documentToDelete = context.t_documento.Include("t_carpeta").Include("t_documento_resp").Include("t_documento_tercero").Where(x => x.id == _selectedDocument.Documento.id).FirstOrDefault();
                                if (documentToDelete != null)
                                {
                                    var carpeta = documentToDelete.t_carpeta;
                                    DeleteDocument(documentToDelete, context);
                                    if (_selectedDocument.RequiereSeleccion) UpdateEstadoCarpeta(carpeta);
                                    await context.SaveChangesAsync();

                                    UpdateView();
                                }
                            }
                        }
                    }
                }
            }
            chkEliminarTodo.IsChecked = false;
            txtMensajeTranscriptores.Text = string.Empty;
        }

        private async void OrdenarResult(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return; //valida si el dialog confirmó dividir el documento

            if (!string.IsNullOrWhiteSpace(txtInicioOrden.Text))
            {
                int inicioOrden = Convert.ToInt32(txtInicioOrden.Text);
                if (1 <= inicioOrden)
                {
                    using (gdocxEntities context = new gdocxEntities())
                    {
                        bool ascendente = rdbtnAscendente.IsChecked.Value;

                        var documentos = context.t_documento
                                        .Include("t_documento_resp.p_tipoitem")
                                        .Include("t_documento_tercero").Where(x => x.t_carpeta.id == _controlCalidadAsignadoModel.id).AsQueryable();

                        documentos = documentos.OrderBy(x => x.pag_ini).Skip(inicioOrden - 1);
                        //orden de pag ini de forma ascendente, skip para el inicio de los documentos

                        var documentosList = documentos.ToList();

                        int count = documentosList.Count; //contador para los items.

                        if (ascendente)
                        {
                            for (int i = 0; i < documentosList.Count; i++)
                            {
                                t_documento documento = documentosList[i];
                                int itemIndex = i + 1;
                                IEnumerable<t_documento_resp> respuestas = documento.t_documento_resp.Where(x => x.p_tipoitem.descripcion.ToLower() == "item").ToList();
                                foreach (var respuesta in respuestas)
                                {
                                    respuesta.valor = itemIndex.ToString(); //cambia la respuesta
                                }
                                documento.item = itemIndex;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < documentosList.Count; i++)
                            {
                                t_documento documento = documentosList[i];
                                int itemIndex = count--;
                                IEnumerable<t_documento_resp> respuestas = documento.t_documento_resp.Where(x => x.p_tipoitem.descripcion.ToLower() == "item").ToList();
                                foreach (var respuesta in respuestas)
                                {
                                    respuesta.valor = itemIndex.ToString(); //cambia la respuesta
                                }
                                documento.item = itemIndex;
                            }
                        }

                        await context.SaveChangesAsync();
                        UpdateView();
                    }
                }
                else
                {
                    MessageBox.Show("El campo de inicio no puede ser menor que 1.");
                }
            }
            else
            {
                MessageBox.Show("El campo inicio del orden no puede estar vacío.");
            }
        }

        private async void DividirResult(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return; //valida si el dialog confirmó dividir el documento
            if (!string.IsNullOrWhiteSpace(txtDividirEn.Text))
            {
                if (_selectedDocument != null)
                {
                    if (_selectedDocument.PagIni == _selectedDocument.PagFin)
                    {
                        MessageBox.Show("No se puede dividir un documento que tiene solamente una página.");
                    }
                    else
                    {
                        int pagFin = -1;
                        try
                        {
                            pagFin = Convert.ToInt32(txtDividirEn.Text);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show($"No se puede convertir {txtDividirEn.Text} a número.");
                        }
                        if (pagFin != -1)
                        {
                            using (gdocxEntities context = new gdocxEntities())
                            {
                                var documento = await context.
                                    t_documento.FindAsync(_selectedDocument.Id); //

                                if (documento.pag_ini <= pagFin
                                    && documento.pag_fin >= pagFin)
                                {
                                    int pagFinOrigen = (int)documento.pag_fin;
                                    documento.pag_fin = pagFin;
                                    await context.SaveChangesAsync();

                                    //se usa asnotracking para evitar tener la misma referencia en la copia del objecto.
                                    var cloned = context.
                                    t_documento.AsNoTracking().Include("t_documento_tercero") //se duplican los terceros
                                    .Include("t_documento_resp") //se duplican las respuestas
                                    .FirstOrDefault(x => x.id == _selectedDocument.Id);

                                    cloned.pag_fin = pagFinOrigen;
                                    cloned.pag_ini = pagFin + 1; //se aumenta en uno donde se divide el documento

                                    context.t_documento.Add(cloned);

                                    int intdex = await context.SaveChangesAsync();

                                    UpdateView();
                                    _reviewedDocuments.Add(new DocumentReview
                                    {
                                        Index = cloned.id,
                                        Reviewed = false
                                    });
                                    txtGridTotalitems.Content = $"Total Documentos: {_reviewedDocuments.Count}";
                                }
                                else
                                {
                                    MessageBox.Show($"La página a dividir no puede ser menor " +
                                        $"que la pagina de inicio, ni mayor que la página final.");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Se requiere la página del documento que se quiere dividir.");
            }
        }

        /// <summary>
        /// Elimina todos los documentos de una carpeta.
        /// </summary>
        /// <param name="carpetaId">Id de la carpeta de los documentos a eliminar.</param>
        /// <returns></returns>
        private async Task RemoveAll(int carpetaId)
        {
            using (gdocxEntities context = new gdocxEntities())
            {
                var dbQuery = context.t_documento.Include("t_carpeta").Include("t_documento_resp").Include("t_documento_tercero").Where(x => x.t_carpeta.id == carpetaId).ToList();
                var rootCarpeta = dbQuery.FirstOrDefault().t_carpeta;

                foreach (var documentToDelete in dbQuery)
                {
                    if (documentToDelete != null)
                    {
                        DeleteDocument(documentToDelete, context);
                    }
                }

                UpdateEstadoCarpeta(rootCarpeta);
                _controlCalidadView.SetTabSelect();
                await context.SaveChangesAsync();

                UpdateView();


            }
        }

        /// <summary>
        /// Actualiza el view y limpia la lista de páginas.
        /// </summary>
        public void UpdateView()
        {
            UpdateWithReset();
            txtMensajeTranscriptores.Text = string.Empty;
            _pages.Clear();
            rowDetailEdit.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Actualiza el estado de la carpeta.
        /// </summary>
        /// <param name="rootCarpeta">Carpeta a actualizar.</param>
        private void UpdateEstadoCarpeta(t_carpeta rootCarpeta)
        {
            rootCarpeta.t_carpeta_estado.Add(new t_carpeta_estado
            {
                rechazado = 1,
                observacion = txtMensajeTranscriptores.Text,
                fase = "D",
                id_usuario = GlobalClass.id_usuario,
                fecha_estado = DateTime.Now
            });
            rootCarpeta.estado = "D";
        }

        /// <summary>
        /// Elimina el documento y sus respuestas.
        /// </summary>
        /// <param name="documentToDelete">Documento a eliminar.</param>
        private static void DeleteDocument(t_documento documentToDelete, gdocxEntities context)
        {
            foreach (var item in documentToDelete.t_documento_resp.ToList()) //elimina las respuestas
            {
                context.t_documento_resp.Remove(item);
            }
            if (documentToDelete.t_documento_tercero.Count != 0)
            {
                foreach (var documentoTercero in documentToDelete.t_documento_tercero.ToList()) //elimina los terceros
                {
                    context.t_documento_tercero.Remove(documentoTercero);
                }
            }
            context.t_documento.Remove(documentToDelete);
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e) => UpdateWithReset();

        /// <summary>
        /// Actualiza la página forzando el pager a recargar los datos del cache.
        /// </summary>
        internal void UpdateWithReset()
        {
            btnActualizar.IsEnabled = false;
            try
            {
                LastItemSelected = -2;
                if (calidadPager.PagedSource != null) //verifica que el pager contiene una lista
                {
                    calidadPager.PagedSource?.ResetCache();
                    if (calidadPager.PageIndex != -1)
                        calidadPager.PagedSource?.ResetCacheForPage(calidadPager.PageIndex);
                    calidadPager.PagedSource?.MoveToPage(calidadPager.PageIndex);
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
            btnActualizar.IsEnabled = true;
        }

        private void CalidadPager_OnDemandLoading(object sender, OnDemandLoadingEventArgs e)
        {
            _userStartIndex = e.StartIndex;
            if (_archivadores == null)
            {
                _archivadores = EntitiesRepository.Entities.p_tipodoc.AsNoTracking().Select(x => x.nombre).Distinct().ToList();
            }
            //int itemsCount = EntitiesRepository.Entities.t_documento.AsNoTracking().Where(x => x.t_documento_resp.Count != 0).Count() / calidadPager.PageSize;
            _baseQueryForReviewCount = BuildControlCalidadGridQuery();

            InitializeReviewCount(_baseQueryForReviewCount.Select(x => x.id).ToList());

            if (!string.IsNullOrEmpty(GlobalClass.SortColumns))
            {
                var sort = GlobalClass.SortColumns.Split(',');
                var isFirst = true;
                foreach (var column in sort)
                {
                    switch (column)
                    {
                        case "item":
                            if (isFirst)
                            {
                                isFirst = false;
                                _baseQueryForReviewCount = _baseQueryForReviewCount.OrderBy(x => x.item);
                            }
                            else
                            {
                                _baseQueryForReviewCount = ((IOrderedQueryable<t_documento>)_baseQueryForReviewCount).ThenBy(x => x.item);
                            }

                            break;
                        case "id":
                            if (isFirst)
                            {
                                isFirst = false;
                                _baseQueryForReviewCount = _baseQueryForReviewCount.OrderBy(x => x.id);
                            }
                            else
                            {
                                _baseQueryForReviewCount = ((IOrderedQueryable<t_documento>)_baseQueryForReviewCount).ThenBy(x => x.id);
                            }
                            break;
                        default:
                            break;
                    }
                }
                _tDocumentResp = QueryToModelList(_baseQueryForReviewCount.Skip(_userStartIndex).Take(calidadPager.PageSize)).ToList();
            }
            else
            {
                _tDocumentResp = QueryToModelList(_baseQueryForReviewCount.OrderBy(x => x.pag_ini).Skip(_userStartIndex).Take(calidadPager.PageSize)).OrderBy(x => x.PagIni).ToList();
            }


            if (_tDocumentResp?.Count != 0 && _userStartIndex != -1)
            {
                int itemsCount = _baseQueryForReviewCount.Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / calidadPager.PageSize);
                calidadPager.PageCount = pageCount;
                calidadPager.LoadDynamicItems(_userStartIndex, _tDocumentResp);
                if (calidadPager.PageIndex != -1)
                {
                    try
                    {
                        calidadPager.PagedSource?.ResetCacheForPage(calidadPager.PageIndex);
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                    }
                }
            }
            else if (_tDocumentResp.Count == 0)
            {
                calidadPager.PageCount = 0;
                calidadPager.LoadDynamicItems(_userStartIndex, _tDocumentResp);
                if (calidadPager.PageIndex != -1)
                    calidadPager.PagedSource?.ResetCacheForPage(calidadPager.PageIndex);
            }

            if (_tDocumentResp.Count == 0)
            {
                _controlCalidadView?.MoveControlCalidadSelect();
                _controlCalidadView?.UpdateWithReset(true);
            }
        }

        private void btnEditarBeneficiarios_Click(object sender, RoutedEventArgs e)
        {
            if (_adminBeneficiarioWindow != null)
            {
                if (_adminBeneficiarioWindow.IsVisible)
                {
                    _adminBeneficiarioWindow.Close();
                }
            }
            _adminBeneficiarioWindow = new AdminBeneficiarioWindow();
            _adminBeneficiarioWindow.SetDocument(_selectedDocument.Documento);
            _adminBeneficiarioWindow.Closed += _adminBeneficiarioWindow_Closed;
            _adminBeneficiarioWindow.Show();
        }

        private async void RangoClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            int tmpDocIni = GlobalClass.GetNumber(txtPagIni.Text);
            int tmpDocFin = GlobalClass.GetNumber(txtPagFin.Text);
            if (tmpDocIni > tmpDocFin)
            {
                MessageBox.Show("La página inicial no puede ser mayor al inicial", "Alpha AI", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (tmpDocFin > 0 && tmpDocIni > 0)
            {
                int ttalDocs = _tDocumentResp.Count;
                int posDoc = 0, tmPagIni = 0, tmPagFin = 0, tmpOldDocIni = 0, tmpOldDocFin = 0;
                for (int d = 0; d < ttalDocs; d++)
                {
                    if (_selectedDocument.Id == _tDocumentResp[d].Id)
                    {
                        posDoc = d;
                        var document = await EntitiesRepository.Entities.t_documento.FindAsync(_tDocumentResp[d].Id);

                        tmpOldDocIni = GlobalClass.GetNumber(document.pag_ini.ToString());
                        tmpOldDocFin = GlobalClass.GetNumber(document.pag_fin.ToString());
                        document.pag_ini = tmpDocIni;
                        document.pag_fin = tmpDocFin;
                        if (tmpDocIni != tmpOldDocIni || tmpDocFin != tmpOldDocFin) await EntitiesRepository.Entities.SaveChangesAsync();
                        d = ttalDocs;   //Sale del FOR
                    }
                }
                ////Revisa el rango anterior
                //if (posDoc > 0)
                //{
                //    //Revisa si tiene más de una página
                //    tmPagIni = _tDocumentResp[posDoc - 1].PagIni;
                //    tmPagFin = _tDocumentResp[posDoc - 1].PagFin;
                //    if ((tmpDocIni - 1) != tmPagFin && (tmpDocIni - 1) >= tmPagIni)
                //    {   //Ajusta el rango Anterior
                //        var document = await EntitiesRepository.Entities.t_documento.FindAsync(_tDocumentResp[posDoc - 1].Id);
                //        document.pag_fin = tmpDocIni-1;
                //        await EntitiesRepository.Entities.SaveChangesAsync();
                //    }
                //}
                ////Revisa el rango siguiente
                //if (posDoc < (ttalDocs-1))
                //{
                //    //Revisa si tiene más de una página
                //    tmPagIni = _tDocumentResp[posDoc + 1].PagIni;
                //    tmPagFin = _tDocumentResp[posDoc + 1].PagFin;
                //    if ((tmpDocFin + 1) != tmPagIni && (tmpDocFin + 1) <= tmPagFin)
                //    {   //Ajusta el rango siguiente
                //        var document = await EntitiesRepository.Entities.t_documento.FindAsync(_tDocumentResp[posDoc +1].Id);
                //        document.pag_ini = tmpDocFin+1;
                //        await EntitiesRepository.Entities.SaveChangesAsync();
                //    }
                //}

                _selectedDocument.PagIni = GlobalClass.GetNumber(txtPagIni.Text);
                _selectedDocument.PagFin = GlobalClass.GetNumber(txtPagFin.Text);
                UpdateWithReset();
            }
            btnAumentarIndex.IsEnabled = false;
            btnDisminuirIndex.IsEnabled = false;
            btnAdminDeleteDocumento.IsEnabled = false;
            btnCambiaRango.IsEnabled = false;
            btnDividir.IsEnabled = false;
        }

        private void _adminBeneficiarioWindow_Closed(object sender, EventArgs e)
        {
            _adminBeneficiarioWindow.Closed -= _adminBeneficiarioWindow_Closed;
            _adminBeneficiarioWindow = null;
        }

        /// <summary>
        /// Obtiene el query básico.
        /// </summary>
        /// <returns></returns>
        private IQueryable<t_documento> GetRootQuery()
        {
            IQueryable<t_documento> queryable = EntitiesRepository.Entities.t_documento/*.AsNoTracking().Include("t_documento_tercero")*/
                                        /*.Include("t_carpeta.t_lote").Include("t_carpeta")*/
                                        /*.Include("t_documento_resp.p_tipoitem")*/
                                        .Include("t_documento_resp.p_tipoitem")
                                        .Include("t_documento_tercero").AsNoTracking().Where(x => x.t_carpeta.id == _controlCalidadAsignadoModel.id).AsQueryable();
            return queryable;
        }

        /// <summary>
        /// Obtiene los documentos visibles al administrador.
        /// </summary>
        /// <returns>IQueryable sin ejecutar.</returns>
        private IQueryable<t_documento> BuildControlCalidadGridQuery()
        {
            var rootQuery = _baseFilteredQuery ?? GetRootQuery();
            return _controlCalidadAsignadoModel != null ? rootQuery : new List<t_documento>().AsQueryable();
        }

        private List<ControlCalidadIListItem> QueryToModelList(IQueryable<t_documento> rootQuery)
        {
            List<ControlCalidadIListItem> queryable = rootQuery.Select(x => new ControlCalidadIListItem
            {
                Asignado = x.t_carpeta.p_usuario1.usuario,
                Estado = x.t_carpeta.estado,
                NoExpediente = x.t_carpeta.nro_expediente,
                NumExpediente = x.t_carpeta.nro_expediente,
                Folios = x.t_carpeta.total_folios ?? 0,
                NoCarpeta = x.t_carpeta.nro_caja,
                CarpetaId = x.t_carpeta.id,
                Archivador = x.p_tipodoc.nombre,
                Respuesta = x.t_documento_resp.ToList(),
                Documento = x,
                Caja = x.t_carpeta.nro_caja,
                Lote = x.t_carpeta.t_lote.nom_lote,
                LoteModel = x.t_carpeta.t_lote,
                PagIni = x.pag_ini ?? -1,
                PagFin = x.pag_fin ?? -1,
                SubSerie = x.t_carpeta.t_lote.id_subserie,
                TotalTerceros = x.t_documento_tercero.Count(),
                RequiereSeleccion = x.requiere_seleccion,
                FolioIni = x.folio_ini,
                FolioFin = x.folio_fin,
            }).ToList();
            for (int i = 0; i < queryable.Count; i++)
            {
                queryable[i].Archivadores = _archivadores;
            }

            return queryable;
        }

        /// <summary>
        /// Carga la lista de elementos totales en control de calidad.
        /// </summary>
        /// <param name="idList">Lista de los id del los documentos.</param>
        private void InitializeReviewCount(List<int> idList)
        {
            if (_totalItemCount == -1)
            {
                _totalItemCount = idList.Count();
                txtGridTotalitems.Content = "Total Documentos: " + _totalItemCount.ToString();
                _reviewedDocuments = new List<DocumentReview>();
                for (int i = 0; i < idList.Count; i++)
                {
                    _reviewedDocuments.Add(new DocumentReview { Index = idList[i], Reviewed = false });
                }
            }
        }

        /// <summary>
        /// Actualiza la imagen seleccionada.
        /// </summary>
        /// <param name="selectedIndex">Index de la imagen seleccionada. -1 por defecto.</param>
        private void UpdateSelectedImage(int selectedIndex = -1)
        {
            if (selectedIndex != -1)
            {
                pdfviewer.CurrentPage = selectedIndex + 1;
            }
        }

        /// <summary>
        /// Actualiza la lista de imágenes cuando el index del grid cambia.
        /// </summary>
        private async void UpdatedGridSelectionAsync(CancellationToken _loadImagesToken)
        {
            if (_selectedDocument != null)
            {
                _cancelToken = _loadImagesToken;
                bool waitLoaded = false;
                try
                {
                    _newPdf = GlobalClass.ruta_proyecto + $@"/{_selectedDocument.Lote}/{_selectedDocument.Caja}/{_selectedDocument.NumExpediente}/{_selectedDocument.NumExpediente}.pdf";
                    if (!File.Exists(_newPdf)) _newPdf = GlobalClass.ruta_proyecto + $@"/{_selectedDocument.Lote}/{_selectedDocument.Caja}/{_selectedDocument.NumExpediente}.pdf";

                    if (File.Exists(_newPdf))
                    {
                        if (_newPdf != _pdfName || !_documenLoaded)
                        {

                            _pdfName = _newPdf;

                            ReleasePdfViewer();
                            _documenLoaded = false;

                            pdfviewer.ToolbarSettings.ShowAnnotationTools = false;
                            pdfviewer.ToolbarSettings.ShowFileTools = false;
                            pdfviewer.ZoomMode = ZoomMode.FitWidth;
                            pdfviewer.WarnBeforeClose = false;


                            var pdfNameCache = CacheHelper.LoadFromCache(_pdfName);

                            waitLoaded = true;
                            _loadedDocument = new PdfLoadedDocument(string.IsNullOrWhiteSpace(pdfNameCache) ? _pdfName : pdfNameCache); //carga el documento desde el disco
                            await pdfviewer.LoadAsync(loadedDocument: _loadedDocument);
                            //_loadedDocument.Close();
                        }
                        else
                        {
                            _pages.Clear();
                        }
                        if (!waitLoaded)
                        {
                            await UpdateOnLoaded();
                        }

                    }
                    else
                    {
                        btnCambiaRango.IsEnabled = false;
                        MessageBox.Show("No se ha encontrado el archivo: \n " + _newPdf);
                    }
                }
                catch (PdfException)
                {
                    btnCambiaRango.IsEnabled = false;
                    MessageBox.Show($"PDF corrupto, error al cargar: {_pdfName}");
                }
                BusyIndicator.IsBusy = false;
            }
        }

        private async Task UpdateOnLoaded()
        {
            controlCalidadTab.SelectedIndex = 1;
            if (_firstLoad)
            {
                //await Task.Delay(1500);
                _firstLoad = false;
            }
            controlCalidadTab.SelectedIndex = 0;
            //int indexDoc = 1;
            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();
            BusyIndicator.IsBusy = true;
            int NumPags = pdfviewer.LoadedDocument.Pages.Count;
            for (int i = (int)(_selectedDocument?.PagIni); i <= _selectedDocument?.PagFin;i++)
            {   //var Pag = pdfviewer.LoadedDocument.ImportPage(pdfviewer.LoadedDocument, i);
                if(_loadImagesToken.IsCancellationRequested){
                    i = (int)(_selectedDocument?.PagFin);
                }
                var exist = _pages.FirstOrDefault(x => x.Index == i.ToString());
                if (exist == null && !_loadImagesToken.IsCancellationRequested && i > 0 && i <= NumPags)
                {
                    BitmapSource image = await Task.Run(() => pdfviewer.ExportAsImage(i - 1, customSize: new SizeF(69f, 114f), false));
                    _pages.Add(new PDFPages
                    {
                        Source = image,
                        Index = i.ToString(),
                        IndexOld = i.ToString()
                    });
                }
            }
            //var RangoPaginas = pdfviewer.LoadedDocument.ImportPageRange(pdfviewer.LoadedDocument, (int)(_selectedDocument?.PagIni), (int)(_selectedDocument?.PagFin));
            //foreach (PdfPageBase page in pdfviewer.LoadedDocument.Pages) //Carga en _pages cada una de las páginas
            //{
            //    if (!_loadImagesToken.IsCancellationRequested &&
            //        _selectedDocument?.PagIni <= indexDoc &&
            //        indexDoc <= _selectedDocument?.PagFin)
            //    {
            //        var exist = _pages.FirstOrDefault(x=>x.Index == indexDoc.ToString());
            //        if (exist == null)
            //        {
            //            BitmapSource image = await Task.Run(() => pdfviewer.ExportAsImage(indexDoc - 1, customSize: new SizeF(69f, 114f), false));
            //            _pages.Add(new PDFPages
            //            {
            //                Source = image,
            //                Index = indexDoc.ToString(),
            //                IndexOld = indexDoc.ToString()
            //            });
            //        }
            //    }
            //    indexDoc++;
            //}
            BusyIndicator.IsBusy = false;
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            if (!_loadImagesToken.IsCancellationRequested)
            {
                //lbxPdfImages.ItemsSource = null;
                lbxPdfImages.ItemsSource = _pages.Distinct().OrderBy(p => p.Index.Length).ThenBy(p => p.Index); ;
                UpdateSelectedImage();
            }
            else
            {
                _pages.Clear();
            }
            if (!_loadImagesToken.IsCancellationRequested)
            {
                txtPagFin.Text = _selectedDocument?.PagFin.ToString();
                txtPagIni.Text = _selectedDocument?.PagIni.ToString();
                btnCambiaRango.IsEnabled = true;
            }
            
        }

        private void ReleasePdfViewer()
        {
            try
            {
                if (_documenLoaded)
                {
                    pdfviewer.Unload(true);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void ControlCalidadGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            var seleccionI = (ControlCalidadIListItem)controlCalidadGrid.SelectedItem;
            if(seleccionI != null)
            {
                if (LastItemSelected == seleccionI.Id) return;
                else LastItemSelected = seleccionI.Id;
            }
            lbxPdfImages.ItemsSource = null;
            //_pages.Clear();


            btnEditarBeneficiarios.IsEnabled = controlCalidadGrid.SelectedIndex != -1;
            //Console.WriteLine("controlCalidadGrid.SelectedItem: " + controlCalidadGrid.SelectedItem);
            if (controlCalidadGrid.SelectedItem != null)
            {
                _selectedDocument = seleccionI;
                //Console.WriteLine("controlCalidadGrid.SelectedIndex: "+controlCalidadGrid.SelectedIndex);
                if (controlCalidadGrid.SelectedIndex != -1)
                {
                    _reviewedDocuments[_reviewedDocuments.FindIndex(x => x.Index == _selectedDocument.Documento.id)].Reviewed = true;
                    BusyIndicator.IsBusy = true;
                    UpdateSelectedDocumentAsync();

                    btnAumentarIndex.IsEnabled = true;
                    btnDisminuirIndex.IsEnabled = true;
                    btnInsertarPdf.IsEnabled = true;
                    btnDividir.IsEnabled = true;
                }
                else if (controlCalidadGrid.SelectedIndex == -1)
                {
                    rowDetailEdit.Visibility = Visibility.Hidden;
                    btnAumentarIndex.IsEnabled = true;
                    btnDisminuirIndex.IsEnabled = true;
                    btnInsertarPdf.IsEnabled = false;
                }
                else
                {
                    btnAumentarIndex.IsEnabled = false;
                    btnDisminuirIndex.IsEnabled = false;
                    btnCambiaRango.IsEnabled = false;
                    btnInsertarPdf.IsEnabled = false;
                    btnDividir.IsEnabled = false;
                }
            }
        }

        internal void SetReviewItem(ControlCalidadAsignadoModel controlCalidadAsignadoModel, ControlCalidadView controlCalidadView, bool fromIndexado = false)
        {
            _totalItemCount = -1;
            rowDetailGroup.Visibility = Visibility.Hidden;
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
                if (pdfviewer.LoadedDocument != null)
                {
                    pdfviewer.LoadedDocument.Dispose();
                    _documenLoaded = false;
                }
            }
            ResetView();
            controlCalidadTab.SelectedIndex = 1;
            controlCalidadTab.SelectedIndex = 0;
            _controlCalidadAsignadoModel = controlCalidadAsignadoModel;
            UpdateWithReset();
            _controlCalidadView = controlCalidadView;
            if (!fromIndexado)
            {
                btnAceptarCarpeta.IsEnabled = controlCalidadAsignadoModel.AsignadoControlCalidad;
            }
            if (fromIndexado)
            {
                btnAceptarCarpeta.IsEnabled = false;
            }
        }

        private void ResetView()
        {
            _pages.Clear();
        }

        /// <summary>
        /// Actualiza el documento seleccionado.
        /// </summary>
        private async void UpdateSelectedDocumentAsync()
        {
            _pages?.Clear();
            try
            {
                Console.WriteLine("953 controlCalidadGrid.SelectedIndex: " + controlCalidadGrid.SelectedIndex);
                if (controlCalidadGrid.SelectedIndex != -1)
                {
                    if (_loadImagesToken != null && _loadImagesToken.CanBeCanceled)
                    {
                        _tokenSource?.Cancel();
                    }
                    var carpMdl = new CarpetaModel { IdSubSerie = _selectedDocument.SubSerie };
                    if (GlobalClass.Carpeta != null)
                    {
                        carpMdl.IdCarptera = GlobalClass.Carpeta.IdCarptera;
                    };

                    GlobalClass.Carpeta = carpMdl;
                    await Task.Run(() => rowDetailEdit.LoadData());

                    if (_selectedDocument != null)
                    {
                        rowDetailEdit.Visibility = Visibility.Visible;
                        rowDetailEdit.SetItem(_selectedDocument); // Actualiza el sub-view con el documento seleccionado
                    }
                    rowDetailGroup.Visibility = Visibility.Visible;

                    _tokenSource = new CancellationTokenSource();
                    _loadImagesToken = _tokenSource.Token;
                    UpdatedGridSelectionAsync(_loadImagesToken);
                }
                btnAdminDeleteDocumento.IsEnabled = _selectedDocument != null;
                btnEditarBeneficiarios.Content = $"Personas ({_selectedDocument.TotalTerceros?.ToString()})";
            }
            catch (Exception ex)
            {
                btnCambiaRango.IsEnabled = false;
                Telemetry.TrackException(ex);
            }
        }

        private void LbxPdfImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxPdfImages.SelectedIndex != -1)
            {
                UpdateSelectedImage(Convert.ToInt32(_pages[lbxPdfImages.SelectedIndex].Index) - 1); //se le pone más uno porque el litsview inicia de 0 y el index de las imágenes desde 1
                controlCalidadTab.SelectedIndex = 1;
                _reviewedDocuments[_reviewedDocuments.FindIndex(x => x.Index == _selectedDocument.Documento.id)].Reviewed = true;
            }
        }

        private async void btnAceptarCarpeta_Click(object sender, RoutedEventArgs e)
        {
            if (_controlCalidadAsignadoModel == null)
            {
                MessageBox.Show("No hay documentos");
                return;
            }
            var documentsToValidate = _reviewedDocuments.Where(x => !x.Reviewed).Select(x => x.Index).ToList();
            if (documentsToValidate.Count != 0)
            {
                MessageBox.Show("No se han verificado todos los documentos, faltan los documentos: " + string.Join(",", documentsToValidate));
            }
            else
            {
                var carpeta = await EntitiesRepository.Entities.t_carpeta.FindAsync(_controlCalidadAsignadoModel.id);
                carpeta.estado = "C";

                EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Histórico Carpeta
                {
                    id_carpeta = carpeta.id,
                    id_usuario = GlobalClass.id_usuario,
                    fase = "C",
                    fecha_estado = DateTime.Now,
                    observacion = $@"Control calidad aceptado",
                });

                await EntitiesRepository.Entities.SaveChangesAsync();
                ResetView();
                _controlCalidadView.controlCalidadSeleccionTab.SelectedIndex = 0;
                _controlCalidadView.UpdateWithReset();
            }
        }

        private void btnDatosLote_Click(object sender, RoutedEventArgs e)
        {
            var carpeta = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Include("t_tercero").Include("t_lote").Include("t_carpeta_estado").Where(x => x.id == _controlCalidadAsignadoModel.id)
               .Select(x => new CarpetaModel
               {
                   t_lote = x.t_lote,
                   nro_caja = x.nro_caja,
                   nro_expediente = x.nro_expediente,
                   nom_expediente = x.nom_expediente,
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
                   tomo_fin = x.tomo_fin,
                   Observaciones = x.kp_observacion
               }).FirstOrDefault();
            if (carpeta != null)
            {
                GlobalClass.Carpeta = carpeta;
                _datosBasicosLote = new DatosBasicosLoteWindow();
                _datosBasicosLote.SetSelectedCarpeta(this, carpeta); //se envían los datos básicos si ya existen.
                _datosBasicosLote.Topmost = true;
                _datosBasicosLote.Show();
            }
            else
            {
                MessageBox.Show("Error al mostrar la carpeta actual.", "Error carpeta", MessageBoxButton.OK);
            }
        }

        private async void btnInsertarPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog opf = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "PDF File (*.pdf)|*.pdf"
                };

                if (opf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string appedFileName = opf.FileName;
                    System.Windows.Forms.DialogResult dialogResult =
                        System.Windows.Forms.MessageBox.Show($"¿Dese insertar {new FileInfo(appedFileName).Name} desde" +
                        $" el index {_selectedDocument.PagFin}?",
                        "Some Title", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (_pages.Count > 0)
                        {
                            ArchivoMeta metaFile = PDFHelper.CopiaPdf(new CarpetaModel
                            {
                                nro_caja = _selectedDocument.Caja,
                                t_lote = _selectedDocument.LoteModel,
                                nro_expediente = _selectedDocument.NoExpediente

                            });  //Hace una copia del archivo Original
                            string pdfOrigen = metaFile.RutaFuente; //Ruta Original
                            string pdfCopia = metaFile.RutaCopia;   //Ruta Copia

                            PdfLoadedDocument oldPdfDoc = new PdfLoadedDocument(pdfCopia); //Abre PDF Copia
                            PdfLoadedDocument appPdfDoc = new PdfLoadedDocument(appedFileName); //Abre PDF a Adicionar
                            PdfDocument doc = new PdfDocument();    //PDF Nuevo

                            int index = 0;
                            foreach (PdfPageBase item in oldPdfDoc.Pages)
                            {
                                doc.ImportPage(oldPdfDoc, index);
                                index++;
                                if (index == _selectedDocument.PagFin)
                                {
                                    int index2 = 0;
                                    foreach (PdfPageBase appendPage in appPdfDoc.Pages)
                                    {
                                        doc.ImportPage(appPdfDoc, index2);
                                        index2++;
                                    }
                                }
                            }
                            
                            doc.Save(metaFile.RutaFuente); //Guarda en la ubicación original
                            doc.Close(true); //Cierra PDF Nuevo

                            var carpeta = await EntitiesRepository.Entities.t_carpeta.FindAsync(_controlCalidadAsignadoModel.id);
                            carpeta.estado = "D";

                            EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Histórico Carpeta
                            {
                                id_carpeta = carpeta.id,
                                id_usuario = GlobalClass.id_usuario,
                                fase = "C",
                                fecha_estado = DateTime.Now,
                                observacion = $@"Se agregó el documento {new FileInfo(appedFileName).Name} desde {_selectedDocument.PagFin}.",
                            });

                            await EntitiesRepository.Entities.SaveChangesAsync();

                            await AumentarPags(appPdfDoc.Pages.Count, showMessages: false, fromInsert: true);

                            ResetView();
                            _controlCalidadView.controlCalidadSeleccionTab.SelectedIndex = 0;
                            _controlCalidadView.UpdateWithReset();

                        }
                    }
                    else if (dialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        MessageBox.Show("Selección cancelada.", "Alpha AI");
                    }
                }
                else
                {
                    MessageBox.Show("Selección cancelada.", "Alpha AI");
                }
            }
            catch (IOException)
            {
                MessageBox.Show("El pdf está en uso, es necesario cerrarlo para poder modificar las páginas.");
            }
        }

        private void btnBorrarCache_Click(object sender, RoutedEventArgs e)
        {
            CacheHelper.DeleteCache();
        }
    }
}