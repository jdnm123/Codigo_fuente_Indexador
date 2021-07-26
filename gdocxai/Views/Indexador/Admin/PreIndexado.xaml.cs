using BertEngine;
using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Models;
using Indexai.OpenCV;
using Indexai.Services;
using Microsoft.ML.Models.BERT.Extensions;
using Syncfusion.Data;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Indexai.Views
{
    /// <summary>
    /// Lógica de interacción para ProcesoOcrPdfa.xaml
    /// </summary>
    public partial class PreIndexadoView : UserControl
    {
        private ObservableCollection<CarpetaModel> _formatoItems;
        private IQueryable<t_carpeta> _baseFilteredQuery;
        private PdfExportService _exportService;
        private List<CarpetaModel> _carpetas;
        private int _userStartIndex;
        private p_trd _tablaPreindexado;
        private BertEngineController _bertController;
        private OpenVCTools _openCvTools;

        public PreIndexadoView()
        {
            InitializeComponent();
            Loaded += FormatosView_Loaded;
            _exportService = new PdfExportService();
            //buscadorView.SetCargaView(this);
            if (GlobalClass.loc_admin == 1) CargarTablas();

            cbxTablaPreindexado.SelectionChanged += CbxTablaPreindexado_SelectionChanged;
            exportPager.OnDemandLoading += ExportPager_OnDemandLoading;
        }

        private void CbxTablaPreindexado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPreIndexar.IsEnabled = e.AddedItems.Count != 0;
            if (e.AddedItems.Count != 0)
            {
                _tablaPreindexado = e.AddedItems[0] as p_trd;
            }
        }

        private void ExportPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            _userStartIndex = e.StartIndex;
            if (_userStartIndex != -1)
            {
                IQueryable<CarpetaModel> query = LoadFormatoItems().AsQueryable();
                _carpetas = query.ToList().Skip(_userStartIndex).Take(exportPager.PageSize).ToList();

                int itemsCount = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Where(x => x.idusr_asignado == GlobalClass.id_usuario && x.estado == "I" && x.t_lote.id_proyecto == GlobalClass.id_proyecto).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / exportPager.PageSize);
                exportPager.PageCount = pageCount != 0 ? pageCount : 1;
                if (_carpetas?.Count != 0)
                {
                    exportPager.LoadDynamicItems(_userStartIndex, _carpetas);
                    if (exportPager.PageIndex != -1)
                    exportPager.PagedSource.ResetCacheForPage(exportPager.PageIndex);
                }
            }
        }

        private void CargarTablas()
        {
            using (gdocxEntities context = new gdocxEntities())
            {
                var trd = EntitiesRepository.Entities.p_trd.AsNoTracking().Include("p_dependencia").Where(x=>x.id_proyecto  == GlobalClass.id_proyecto).ToList();
                cbxTablaPreindexado.ItemsSource = trd;
            }
        }

        /// <summary>
        /// Aplica el filtro al grid desde el buscador.
        /// </summary>
        /// <param name="codCarpeta"></param>
        /// <param name="expediente"></param>
        /// <param name="nomLote"></param>
        /// <param name="numCaja"></param>
        /// <param name="nombreUsuario"></param>
        internal void SetFilter(string codCarpeta, string expediente, string nomLote, string numCaja, string nombreUsuario, string txtNroCarpeta, int iniPag, int maxPag)
        {
            _baseFilteredQuery = GetRootQuery().AsQueryable();


            //REVISAR
            if (!string.IsNullOrWhiteSpace(codCarpeta))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_expediente == codCarpeta);
            }
            if (!string.IsNullOrWhiteSpace(expediente))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nom_expediente == expediente);
            }
            if (!string.IsNullOrWhiteSpace(nomLote))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.t_lote.ToString() == nomLote);
            }
            if (!string.IsNullOrWhiteSpace(numCaja))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_caja == numCaja);
            }
            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                var userIds = EntitiesRepository.Context.p_usuario.AsNoTracking().Where(x => x.usuario.Contains(nombreUsuario)).Select(x => x.id).ToList();
                _baseFilteredQuery = _baseFilteredQuery.Where(x => userIds.Any(y => y == x.idusr_asignado) || userIds.Any(y => y == x.idusr_control));
            }
            if (!string.IsNullOrWhiteSpace(txtNroCarpeta))
            {
                int NoKP = GlobalClass.GetNumber(txtNroCarpeta);
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_carpeta == NoKP);
            }
            if (0 <= iniPag)
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.int_caja >= iniPag);
            }
            if (0 <= maxPag)
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.int_caja <= maxPag);
            }
            UpdateWithReset();

            UpdatePage();
        }

        private void UpdateWithReset()
        {
            try
            {
                if (exportPager.PagedSource != null) //verifica que el pager contiene una lista
                {
                    exportPager.PagedSource.ResetCache();
                    if (exportPager.PageIndex != -1)
                    exportPager.PagedSource.ResetCacheForPage(exportPager.PageIndex);
                    exportPager.PagedSource.MoveToPage(exportPager.PageIndex);
                }
                else
                {
                    UpdatePage(true);
                }

            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Actializa la página actual.
        /// </summary>
        private void UpdatePage(bool removeCache = false, bool ambas = false)
        {
            if (exportPager.PagedSource != null)
            {
                IQueryable<CarpetaModel> query = LoadFormatoItems().AsQueryable();
                _carpetas = query.ToList().Skip(exportPager.PageIndex).Take(exportPager.PageSize).ToList();

                int itemsCount = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Where(x => x.idusr_asignado == GlobalClass.id_usuario && x.estado == "I" && x.t_lote.id_proyecto == GlobalClass.id_proyecto).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / exportPager.PageSize);
                exportPager.PageCount = pageCount != 0 ? pageCount : 1;
                if (_carpetas?.Count != 0)
                {
                    exportPager.LoadDynamicItems(_userStartIndex, _carpetas);
                    if (removeCache)
                    if (exportPager.PageIndex != -1)
                        exportPager.PagedSource.ResetCacheForPage(exportPager.PageIndex);
                }
            }
        }

        private void FormatosView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTabFormato();
            buscador.SetPreindexadoView(this);
            UpdateWithReset();

            UpdatePage();
            exportPager.MoveToFirstPage();
            
        }

        private void LoadTabFormato()
        {
            exportPager.PageSize = 50;
            if (_formatoItems == null)
            {
                UpdateView();
            }
            exportPager.Source = _formatoItems;
        }

        internal void UpdateView()
        {
            _formatoItems = LoadFormatoItems();
            exportPager.Source = _formatoItems;
            txtGridTotalitems.Content = "Total registros: " + _formatoItems.Count().ToString("###.###.###");
        }

        private ObservableCollection<CarpetaModel> LoadFormatoItems()
        {
            return new ObservableCollection<CarpetaModel>(
                GetRootQuery().Select(x => new CarpetaModel
                {
                    t_lote = x.t_lote,
                    nro_caja = x.nro_caja,
                    nro_expediente = x.nro_expediente,
                    nom_expediente = x.nom_expediente,
                    nro_carpeta = x.nro_carpeta ?? 0,
                    Folios = x.total_folios ?? 0,
                    Paginas = x.paginas ?? 0,
                    Asignado = x.p_usuario1.usuario,
                    Estado = x.estado,
                    IdUsuario = x.idusr_asignado,
                    NoExpediente = x.nro_expediente.ToString(),
                    IdSubSerie = x.t_lote.id_subserie ?? -1,
                    IdCarptera = x.id,
                    IdTercero = x.id_tercero
                }).ToList()
            );
        }

        private void BtnMaximoItemsGrid_Click(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private IQueryable<t_carpeta> GetRootQuery()
        {
            var rootQuery = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.estado == "P" && x.t_lote.id_proyecto == GlobalClass.id_proyecto).AsQueryable();

            
            rootQuery = buscador.GetQueryFilter(rootQuery);

            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(txtItemCount.Text))
                {
                    rootQuery = rootQuery.Take(Convert.ToInt32(txtItemCount.Text));
                }
            });
            return rootQuery;
        }

        private async void btnPreIndexar_Click(object sender, RoutedEventArgs e)
        {
            var tdr = EntitiesRepository.Entities.p_docimg_tipodoc.Include("p_tipodoc").Include("p_docimg").Where(x =>
            x.p_tipodoc.p_subserie.p_serie.p_subdependencia.p_dependencia.p_trd.id_proyecto
            == GlobalClass.id_proyecto).ToDictionary(x=>
            StringExtensions.CleanNames(x.p_docimg.nombre_img), x=>x);

            string bertModelFile = "indexador-bert.onnx";
            string bertVocabFile = "vocab.txt";
            if (_bertController == null)
            {
                if (!File.Exists(bertModelFile))
                {
                    throw new FileNotFoundException($"El archivo {bertModelFile} no existe.");
                }
                if (!File.Exists(bertVocabFile))
                {
                    throw new FileNotFoundException($"El archivo {bertVocabFile} no existe.");
                }
                await Task.Run(() =>
                {
                   _bertController = new BertEngineController();
                    _bertController.LoadModel(bertModelFile, bertVocabFile);
                });
            }
            if (_openCvTools == null)
            {
                _openCvTools = new OpenVCTools();
                await Task.Run(() =>
                {
                    _openCvTools.LoadTemplates();
                });
            }

            

            foreach (var document in _formatoItems)
            {
                var preIndexados = EntitiesRepository.Context.t_documento.Where(x => x.t_carpeta.id == document.IdCarptera).OrderBy(x=>x.pag_fin).ToList();

                string ruta = GlobalClass.ruta_proyecto + $@"/{document.t_lote.nom_lote}/{document.nro_caja}/{document.nro_expediente}/{document.nro_expediente}.pdf";
                if (!File.Exists(ruta))
                {
                    ruta = GlobalClass.ruta_proyecto + $@"/{document.t_lote.nom_lote}/{document.nro_caja}/{document.nro_expediente}.pdf";
                }

                var doc = new PdfLoadedDocument(ruta);
                
                int index = 0;
                foreach (PdfLoadedPage page in doc.Pages)
                {
                    var image = doc.ExportAsImage(index, 250, 250);
                    var text = page.ExtractText();
                    
                    string clean = TextCleaner.CleanBert(text);
                    
                    var (classType, probability) = _bertController.Predict(clean);

                    if (_openCvTools.ProcessImage(image, classType))
                    {
                        if (tdr.ContainsKey(classType))
                        {
                            var documentoExistente = preIndexados
                                .FirstOrDefault(x=>x.pag_ini == index && x.pag_fin == index);
                            if (documentoExistente == null)
                            {
                                var type = tdr[classType];
                                t_documento documento = new t_documento
                                {
                                    id_tipodoc = type.p_tipodoc.id,
                                    id_carpeta = document.IdCarptera,
                                    pag_ini = index,
                                    pag_fin = index,
                                    fecha_regdoc = DateTime.Now,
                                    requiere_seleccion = true,
                                    preindexado = true,
                                };
                                //EntitiesRepository.Entities.t_documento.Add(documento);
                                EntitiesRepository.Context.t_documento.Add(documento);
                                var wef = await EntitiesRepository.Context.SaveChangesAsync();
                            }
                        }
                    }
                    index++;
                }
            }
        }
    }
}