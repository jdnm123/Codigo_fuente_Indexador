using Indexai.Models;
using Indexai.Services;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using UserControl = System.Windows.Controls.UserControl;
using System.ComponentModel;
using Gestion.DAL;
using System.Data.Entity;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using Indexai.Helpers;

namespace Indexai.Views
{
    /// <summary>
    /// Interaction logic for IndexacionView.xaml
    /// </summary>
    public partial class ControlCalidadView : UserControl
    {
        private IQueryable<t_carpeta> _baseFilteredQuery;
        private int _publicosStartIndex = 0;
        private int _userStartIndex = 0;
        private List<ControlCalidadAsignadoModel> _userItems;
        private List<ControlCalidadAsignadoModel> _publicItems;
        private bool _sortFechaIndexado;
        private ListSortDirection _sortFechaIndexadoDirection;
        private bool _fromIndexado;

        public int? AsignadoControlCalidad { get; private set; }

        public ControlCalidadView()
        {
            InitializeComponent();
            Loaded += ControlCalidadView_Loaded;
            controlCalidadCarpetas.SelectedIndexChanged += ControlCalidadCarpetas_SelectedIndexChanged;
            buscadorViewCalidad.SetControlCalidadView(this);
            calidadPublicosPager.OnDemandLoading += CalidadPublicosPager_OnDemandLoading;
            controlCalidadUsuarioPager.OnDemandLoading += ControlCalidadUsuarioPager_OnDemandLoading;
            controlCalidadSeleccionTab.SelectionChanged += ControlCalidadSeleccionTab_SelectionChanged;
            dlgEditCaja.DialogOpened += dlgEditCaja_OnDialogOpened;
        }

        internal void ShowPdfViewer()
        {
            controCalidadReview.ShowPdfViewer();
        }

        private void ControlCalidadSeleccionTab_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (controlCalidadSeleccionTab.SelectedIndex == 0)
            {
                controCalidadReview.MoveFirstPage();
            }    
        }

        private void ControlCalidadCarpetas_SelectedIndexChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // UpdatePage(true);
            UpdateWithReset(true);
        }

        private void ControlCalidadView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //UpdatePage(true, true); 
            //UpdateWithReset(true);
            calidadPublicosPager.MoveToFirstPage();
            controlCalidadUsuarioPager.MoveToFirstPage();
            //if (!_fromIndexado) 
            if (GlobalClass.FromIndexado)
            {
                TabCCLista.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                TabCCLista.Visibility = System.Windows.Visibility.Visible;
                controlCalidadSeleccionTab.SelectedIndex = 0;
            }
        }

        internal void CancelControlCalidad()
        {
            controCalidadReview.Cancel();
            tabControlCalidadParent.Visibility = System.Windows.Visibility.Hidden;
            controlCalidadSeleccionTab.SelectedIndex = 0;
        }

        /// <summary>
        /// Registra los cambios de index en los grids y carga los lotes.
        /// </summary>
        private void FillGrids()
        {
            controlCalidadUsuarioGrid.SelectionChanged += IndexadorUserSelectionChanged;
            calidadPublicosGrid.SelectionChanged += IndexadorGridPublic_SelectionChanged;

            //if (_lotesPublicos != null)
            //{
            //    _lotesPublicos = new ObservableCollection<CarpetaModel>(_lotesPublicos.Where(x => !x.IdUsuario.HasValue).ToList());
            //}
            //if (_misLotes != null)
            //{
            //    _misLotes = new ObservableCollection<CarpetaModel>(_misLotes.Where(x => GlobalClass.id_usuario == x.IdUsuario).ToList());
            //}
        }

        private void ControlCalidadUsuarioPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            _userStartIndex = e.StartIndex;
            UpdateUserPage();
        }

        /// <summary>
        /// Actualiza las páginas del usuario.
        /// </summary>
        private void UpdateUserPage()
        {

            if (_userStartIndex != -1)
            {
                IQueryable<t_carpeta> query = BuildControlCalidadUserGridQuery();
                IOrderedQueryable<t_carpeta> orderBy = _sortFechaIndexado ?
                    (_sortFechaIndexadoDirection == ListSortDirection.Ascending ? query.OrderBy(x => x.fecha_indexa) : query.OrderByDescending(x => x.fecha_indexa)):
                    query.OrderByDescending(x => x.fecha_indexa);

                IQueryable<t_carpeta> nonSkip = orderBy.Where(x => x.estado == "I" && x.t_lote.id_proyecto == GlobalClass.id_proyecto &&
                                 (x.idusr_control == GlobalClass.id_usuario || x.id_usuario == GlobalClass.id_usuario));

                 nonSkip = _sortFechaIndexado ?
                    (_sortFechaIndexadoDirection == ListSortDirection.Ascending ? nonSkip.OrderBy(x => x.fecha_indexa) : nonSkip.OrderByDescending(x => x.fecha_indexa)) :
                    nonSkip.OrderByDescending(x => x.fecha_indexa);

                _userItems = nonSkip.Skip(_userStartIndex).Take(controlCalidadUsuarioPager.PageSize).Select(x => new ControlCalidadAsignadoModel
                {
                    id = x.id,
                    FechaIndexado = x.fecha_indexa,
                    Lote = x.t_lote.nom_lote,
                    NomExpediente = x.nom_expediente,
                    NroCarpeta = x.nro_carpeta ?? 0,
                    hc_fin = x.hc_fin,
                    TotalFolios = x.total_folios ?? 0,
                    NroExpediente = x.nro_expediente,
                    Caja = x.nro_caja,
                    AsignadoControlCalidad = x.idusr_control != null,
                    Realizo = x.p_usuario.usuario, //p_usuario = id_usuario,p_usuario1 = idusr_asignado, p_usuario2 = idusr_control
                }).ToList();

                if (!_sortFechaIndexado)
                {
                    _userItems = _userItems.OrderByDescending(x => x.FechaIndexado).ToList();
                }

                int itemsCount = nonSkip.Count(); /*EntitiesRepository.Entities.t_carpeta.AsNoTracking().Where(x => x.estado == "I" && x.t_lote.id_proyecto == GlobalClass.id_proyecto && (x.idusr_control == GlobalClass.id_usuario || x.id_usuario == GlobalClass.id_usuario)).Count()*/;
                int pageCount = (int)Math.Ceiling((double)itemsCount / controlCalidadUsuarioPager.PageSize);
                txtGridTotalUser.Content = "Total registros: " + itemsCount.ToString("###.###.###");
                controlCalidadUsuarioPager.PageCount = pageCount != 0 ? pageCount : 1;
                if (_userItems?.Count == 0)
                {
                    controlCalidadUsuarioPager.PageCount = 0;
                }
                try
                {
                    controlCalidadUsuarioPager.LoadDynamicItems(_userStartIndex, _userItems);
                }
                catch (Exception)
                {

                }
                try
                {
                    if (controlCalidadUsuarioPager.PageIndex != -1)
                        controlCalidadUsuarioPager.PagedSource?.ResetCacheForPage(controlCalidadUsuarioPager.PageIndex);
                }
                catch (Exception)
                {


                }
            }
        }

        private void CalidadPublicosPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            _publicosStartIndex = e.StartIndex;
            UpdatePublicPage();
        }

        /// <summary>
        /// Actualiza las páginas públicas.
        /// </summary>
        private void UpdatePublicPage()
        {
            if (_publicosStartIndex != -1)
            {
                IQueryable<t_carpeta> query = BuildControlCalidadPublicGridQuery();
                IQueryable<t_carpeta> nonSkip = query.OrderBy(x => x.nro_carpeta).Where(x => x.estado == "I" && x.t_lote.id_proyecto == GlobalClass.id_proyecto && (x.idusr_asignado == null || x.id_usuario == null));
                _publicItems = nonSkip.Skip(_publicosStartIndex).Take(calidadPublicosPager.PageSize).Select(x => new ControlCalidadAsignadoModel
                {
                    id = x.id,
                    FechaIndexado = x.fecha_indexa,
                    Lote = x.t_lote.nom_lote,
                    NomExpediente = x.nom_expediente,
                    NroCarpeta = x.nro_carpeta ?? 0,
                    hc_fin = x.hc_fin,
                    TotalFolios = x.total_folios ?? 0,
                    NroExpediente = x.nro_expediente,
                    Caja = x.nro_caja
                }).ToList();

                int itemsCount = nonSkip.Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / calidadPublicosPager.PageSize);
                txtGridTotalPublicitems.Content = "Total registros: " + itemsCount.ToString("###.###.###");
                calidadPublicosPager.PageCount = pageCount != 0 ? pageCount : 1;
                if (_publicItems?.Count == 0)
                {
                    controlCalidadUsuarioPager.PageCount = 0;
                }
                try
                {
                    calidadPublicosPager.LoadDynamicItems(_publicosStartIndex, _publicItems);
                }
                catch (Exception)
                {
                }
                try
                {
                    if (calidadPublicosPager.PageIndex != -1)
                        calidadPublicosPager.PagedSource?.ResetCacheForPage(calidadPublicosPager.PageIndex);
                }
                catch (Exception)
                {


                }
            }
        }

        private void IndexadorUserSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IndexadorGridPublic_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene el query básico.
        /// </summary>
        /// <returns></returns>
        private static IQueryable<t_carpeta> GetRootQuery()
        {
            return EntitiesRepository.Entities.t_carpeta.AsNoTracking();
        }

        /// <summary>
        /// Elimina el filtro de búsqueda
        /// </summary>
        internal void RemoveFilter()
        {
            _baseFilteredQuery = null;
            UpdatePage(true);
            UpdateWithReset();
        }

        /// <summary>
        /// Actualiza la página forzando el pager a recargar los datos del cache.
        /// </summary>
        internal void UpdateWithReset(bool ambas = false)
        {
            tabControlCalidadParent.Visibility = System.Windows.Visibility.Hidden;
            btnActualizar.IsEnabled = false;
            try
            {
                if (controlCalidadCarpetas.SelectedIndex == 0 || ambas)
                {
                    if (controlCalidadUsuarioPager.PagedSource != null) //verifica que el pager contiene una lista
                    {
                        controlCalidadUsuarioPager.PagedSource?.ResetCache();
                        if (controlCalidadUsuarioPager.PageIndex != -1)
                            controlCalidadUsuarioPager.PagedSource?.ResetCacheForPage(controlCalidadUsuarioPager.PageIndex);
                        controlCalidadUsuarioPager.PagedSource?.MoveToPage(controlCalidadUsuarioPager.PageIndex);
                    }
                    else
                    {
                        UpdatePage(true, ambas);
                    }
                }
                if (controlCalidadCarpetas.SelectedIndex == 1 || ambas)
                {
                    if (calidadPublicosPager.PagedSource != null) //verifica que el pager contiene una lista
                    {
                        calidadPublicosPager.PagedSource?.ResetCache();
                        if (calidadPublicosPager.PageIndex != -1)
                            calidadPublicosPager.PagedSource?.ResetCacheForPage(calidadPublicosPager.PageIndex);
                        calidadPublicosPager.PagedSource?.MoveToPage(calidadPublicosPager.PageIndex);
                    }
                    else
                    {
                        UpdatePage(true, ambas);
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
            btnActualizar.IsEnabled = true;
        }

        internal void SetTabSelect()
        {
            controlCalidadSeleccionTab.SelectedIndex = 0;
            UpdateWithReset();
        }

        /// <summary>
        /// Actializa la página actual.
        /// </summary>
        private void UpdatePage(bool removeCache = false, bool ambas = false)
        {
            if (controlCalidadCarpetas.SelectedIndex == 0 || ambas)
            {
                UpdateUserPage();
            }
            if (controlCalidadCarpetas.SelectedIndex == 1 || ambas)
            {
                UpdatePublicPage();
            }
        }

        /// <summary>
        /// Mueve control de calidad a la selección de carpeta.
        /// </summary>
        internal void MoveControlCalidadSelect()
        {
            controlCalidadSeleccionTab.SelectedIndex = 0;
        }

        /// <summary>
        /// Obtiene los documentos visibles al administrador.
        /// </summary>
        /// <returns>IQueryable sin ejecutar.</returns>
        private IQueryable<t_carpeta> BuildControlCalidadUserGridQuery(bool OfQueryable = false)
        {
            return _baseFilteredQuery ?? GetRootQuery();
        }

        /// <summary>
        /// Obtiene los documentos visibles al administrador.
        /// </summary>
        /// <returns>IQueryable sin ejecutar.</returns>
        private IQueryable<t_carpeta> BuildControlCalidadPublicGridQuery(bool OfQueryable = false)
        {
            return _baseFilteredQuery ?? GetRootQuery();
        }


        /// <summary>
        /// Aplica el filtro al grid desde el buscador.
        /// </summary>
        /// <param name="codCarpeta"></param>
        /// <param name="expediente"></param>
        /// <param name="nomLote"></param>
        /// <param name="numCaja"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="nroExpediente"></param>
        internal void SetFilter(string codCarpeta, string expediente, string nomLote, string numCaja, string nombreUsuario, string txtNroCarpeta, int iniPag, int maxPag, DateTime? dateIndex)
        {
            _baseFilteredQuery = controlCalidadSeleccionTab.SelectedIndex == 0 ? BuildControlCalidadUserGridQuery().AsQueryable() : BuildControlCalidadPublicGridQuery().AsQueryable();
            //REVISAR
            if (dateIndex != new DateTime(0001,1,1))
            {
                var endDay = new DateTime(dateIndex.Value.Year, dateIndex.Value.Month, dateIndex.Value.Day).AddDays(1);
                var startDate = new DateTime(dateIndex.Value.Year, dateIndex.Value.Month, dateIndex.Value.Day);
                _baseFilteredQuery = _baseFilteredQuery.Where(x => (x.fecha_indexa >= startDate) && (x.fecha_indexa < endDay));
            }
            if (!string.IsNullOrWhiteSpace(codCarpeta))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_expediente.Contains(codCarpeta));
            }
            if (!string.IsNullOrWhiteSpace(expediente))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nom_expediente.Contains(expediente));
            }
            if (!string.IsNullOrWhiteSpace(nomLote))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.t_lote.nom_lote.ToString().Contains(nomLote));
            }
            if (!string.IsNullOrWhiteSpace(numCaja))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_caja.Equals(numCaja));
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
        }

        private void btnActualizar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateWithReset(false);
        }

        private void controlCalidadUsuarioGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            var selectedItem = (ControlCalidadAsignadoModel)controlCalidadUsuarioGrid.SelectedItem;
            if (selectedItem != null)
            {
                controlCalidadSeleccionTab.SelectedIndex = 1;
                ControlCalidadAsignadoModel carpeta = GetCarpeta(selectedItem.id);
                controCalidadReview.SetReviewItem(carpeta, this);
            }
        }

        private static ControlCalidadAsignadoModel GetCarpeta(int carpetaId)
        {
            return EntitiesRepository.Entities.t_carpeta.Where(x => x.id == carpetaId).Select(x => new ControlCalidadAsignadoModel
            {
                id = x.id,
                Lote = x.t_lote.nom_lote,
                NomExpediente = x.nom_expediente,
                NroCarpeta = x.nro_carpeta ?? 0,
                hc_fin = x.hc_fin,
                TotalFolios = x.total_folios ?? 0,
                NroExpediente = x.nro_expediente,
                Caja = x.nro_caja,
                AsignadoControlCalidad = x.idusr_control != null
            }).FirstOrDefault();
        }

        private void calidadPublicosGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = (ControlCalidadAsignadoModel)calidadPublicosGrid.SelectedItem;
            
            if (selectedItem != null)
            {
                int idCarpeta = selectedItem.id;
                SetCarpeta(idCarpeta);
            }
        }

        /// <summary>
        /// Cambia control calidad a editar la carpeta.
        /// </summary>
        /// <param name="idCarpeta">Id de la carpeta.</param>
        public void SetCarpeta(int idCarpeta, bool fromIndexado = false)
        {
            tabControlCalidadParent.Visibility = System.Windows.Visibility.Visible;
            ControlCalidadAsignadoModel carpeta = GetCarpeta(idCarpeta);
            controCalidadReview.SetReviewItem(carpeta, this, fromIndexado);
            tabControlCalidadParent.IsSelected = true;
            _fromIndexado = fromIndexado;
            controlCalidadSeleccionTab.SelectedIndex = 0;
            controlCalidadSeleccionTab.SelectedIndex = 1;
        }

        private void dlgEditCaja_OnDialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            txtCaja.Text = string.Empty;
        }

        private void CambiarcajaClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            if (!string.IsNullOrEmpty(txtCaja.Text))
            {
                int Registros = _userItems.Count();
                string nuevaCaja = txtCaja.Text;
                DialogResult result = MessageBox.Show($@"Seguro que desea mover {Registros} Carpetas a la Caja {nuevaCaja}?", "Cambiar Caja", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Console.WriteLine(_userItems.Count());
                    foreach (ControlCalidadAsignadoModel c in _userItems)
                    {
                        string cajaAnterior = c.Caja;
                        var Sql = "update t_carpeta set nro_caja = '" + nuevaCaja + "' where id = " + c.id;
                        var resultado = EntitiesRepository.Context.Database.ExecuteSqlCommand(Sql);
                        if (resultado == 1)
                        {
                            try
                            {
                                string fileIni = GlobalClass.ruta_proyecto + $@"/{c.Lote}/{cajaAnterior}/{c.NroExpediente}/{c.NroExpediente}.pdf";
                                if (!File.Exists(fileIni)) fileIni = GlobalClass.ruta_proyecto + $@"/{c.Lote}/{cajaAnterior}/{c.NroExpediente}.pdf";
                                if (File.Exists(fileIni))
                                {
                                    string fileFin = GlobalClass.ruta_proyecto + $@"/{c.Lote}/{nuevaCaja}/{c.NroExpediente}.pdf";
                                    Directory.CreateDirectory(GlobalClass.ruta_proyecto + $@"/{c.Lote}/{nuevaCaja}/");
                                    File.Copy(fileIni, fileFin, true);
                                    if (File.Exists(fileFin)) File.Delete(fileIni);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    if (Registros>0) UpdateWithReset(false);
                }
                else
                {
                    eventArgs.Cancel();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Debe digitar un nuevo Número de Caja");
                eventArgs.Cancel();
                return;
            }
        }

        private void controlCalidadUsuarioGrid_SortColumnsChanged(object sender, GridSortColumnsChangedEventArgs e)
        {
            _sortFechaIndexado = false;
            foreach (var sortColumn in controlCalidadUsuarioGrid.View.SortDescriptions)
            {
                if (sortColumn.PropertyName == "FechaIndexadoFormat")
                {
                    _sortFechaIndexado = true;
                    _sortFechaIndexadoDirection = sortColumn.Direction;
                }
            }
            UpdatePage(true);
            UpdateWithReset();
        }

        private void btnActualizarCache_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btnActualizar.IsEnabled = false;
            pbarUpdateCache.Visibility = System.Windows.Visibility.Visible;
            int currentIndex = controlCalidadCarpetas.SelectedIndex;
            Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (currentIndex == 0)
                        {
                            pbarUpdateCache.Maximum = _userItems.Count;
                            foreach (var carpeta in _userItems)
                            {
                                SaveToCache(carpeta);
                                pbarUpdateCache.Value++;
                            }
                        }
                        else
                        {
                            pbarUpdateCache.Maximum = _publicItems.Count;
                            foreach (var carpeta in _publicItems)
                            {
                                SaveToCache(carpeta);
                                pbarUpdateCache.Value++;
                            }
                        }
                    });
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                finally
                {
                    Dispatcher.Invoke(() => pbarUpdateCache.Visibility = System.Windows.Visibility.Collapsed);
                    Dispatcher.Invoke(() => btnActualizar.IsEnabled = true);
                }
            });
        }
        private static string GetRuta(ControlCalidadAsignadoModel carptea)
        {
            string ruta = GlobalClass.ruta_proyecto + $@"/{carptea.Lote}/{carptea.Caja}/{carptea.NroExpediente}/{carptea.NroExpediente}.pdf";
            if (!File.Exists(ruta))
            {
                ruta = GlobalClass.ruta_proyecto + $@"/{carptea.Lote}/{carptea.Caja}/{carptea.NroExpediente}.pdf";
            }

            return ruta;
        }
        private void SaveToCache(ControlCalidadAsignadoModel carptea)
        {
            string ruta = string.Empty;
            ruta = GetRuta(carptea);
            CacheHelper.SaveToCacheAsync(ruta);
        }
    }
}