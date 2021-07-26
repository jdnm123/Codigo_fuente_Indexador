using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Models;
using Indexai.Services;
using MaterialDesignThemes.Wpf;
using Syncfusion.Pdf;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Indexai.Views
{
    /// <summary>
    /// Lógica de interacción para ExportarView.xaml
    /// </summary>
    public partial class ExportarView : System.Windows.Controls.UserControl
    {
        private ObservableCollection<CarpetaModel> _formatoItems;
        private IQueryable<t_carpeta> _baseFilteredQuery;
        private PdfSplitService _exportService;
        private int idcarpetaSeleccion;
        private DatosBasicosLoteWindow _datosBasicosLote;

        public ExportarView()
        {
            InitializeComponent();
            //if (GlobalClass.loc_admin == 1 || GlobalClass.loc_calidad == 1) {
            Loaded += FormatosView_Loaded;
            exportGrid.SelectionChanged += exportGrid_SelectionChanged;
            _exportService = new PdfSplitService();
            buscadorView.SetFormatos(this);
            //}
        }

        private async void exportGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            try
            {
                if (exportGrid.SelectedItems.Count == 1)
                {
                    CarpetaModel selectedItem = (CarpetaModel)exportGrid.SelectedItem;
                    if (selectedItem != null)
                    {
                        idcarpetaSeleccion = selectedItem.IdCarptera;
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        private async void FormatosView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTabFormatoAsync();
        }

        private async Task LoadTabFormatoAsync()
        {
            exportPager.PageSize = 50;
            if (_formatoItems == null)
            {
                await UpdateView();
            }
            exportPager.Source = _formatoItems;
        }

        public async Task UpdateView()
        {
            _formatoItems = LoadFormatoItems();
            txtGridTotalitems.Content = "Total Documentos: " + _formatoItems.Count.ToString();
            Dispatcher.Invoke(() => exportPager.Source = _formatoItems);
        }

        //private async void BtnExportarCarpeta_Click(object sender, RoutedEventArgs e)
        //{

        //}


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
                    IdTercero = x.id_tercero,
                    NroCaja = (int)x.int_caja
                }).Take(3000).ToList()
            );
        }

        /// <summary>
        /// Elimina el filtro de búsqueda
        /// </summary>
        internal void RemoveFilter()
        {
            _baseFilteredQuery = null;
            UpdateView();
        }

        private IQueryable<t_carpeta> GetRootQuery()
        {
            var rootQuery = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.t_lote.id_proyecto == GlobalClass.id_proyecto && (x.estado == "C")).AsQueryable();

            rootQuery = buscadorView.GetQueryFilter(rootQuery);

            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(txtItemCount.Text))
                {
                    rootQuery = rootQuery.Take(Convert.ToInt32(txtItemCount.Text));
                }
            });
            return rootQuery;
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
            _baseFilteredQuery = GetRootQuery();
            if (dateIndex != new DateTime(0001, 1, 1))
            {
                var endDay = new DateTime(dateIndex.Value.Year, dateIndex.Value.Month, dateIndex.Value.Day).AddDays(1);
                var startDate = new DateTime(dateIndex.Value.Year, dateIndex.Value.Month, dateIndex.Value.Day);
                _baseFilteredQuery = _baseFilteredQuery.Where(x => (x.fecha_indexa >= startDate) && (x.fecha_indexa < endDay));
            }
            if (!string.IsNullOrWhiteSpace(codCarpeta))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_caja.Contains(codCarpeta));
            }
            if (!string.IsNullOrWhiteSpace(expediente))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nom_expediente.Contains(expediente));
            }
            if (!string.IsNullOrWhiteSpace(nomLote))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.t_lote.nom_lote.Contains(nomLote));
            }
            if (!string.IsNullOrWhiteSpace(numCaja))
            {
                _baseFilteredQuery = _baseFilteredQuery.Where(x => x.nro_caja.Contains(numCaja));
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
        }

        private async void btnMaximoItemsGrid_Click(object sender, RoutedEventArgs e)
        {
            await UpdateView();
        }

        private async void DlgExportar_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            bool hoja_control = false;
            bool sobreescribir = false;
            int ocr = 0;
            //cambiar aquí por el query querido para la nueva función
            exportPBar.Value = 0;
            hoja_control = (bool)chkHojaControl.IsChecked;
            sobreescribir = (bool)chkSobreescribir.IsChecked;

            var numOCR = EntitiesRepository.Entities.p_proyecto.Where(x => x.id == GlobalClass.id_proyecto).Select(x => x.nivel_ocr).FirstOrDefault().ToString();
            ocr = GlobalClass.GetNumber(numOCR);
            IEnumerable<CarpetaModel> toExport = _formatoItems.DistinctBy(m => new { m.NoExpediente, m.nom_expediente }).OrderBy(x => x.NoExpediente).ThenBy(x => x.nom_expediente);
            exportPBar.Maximum = toExport.Count();
            List<string> exportedList = new List<string>();

            await Task.Run(async () =>
            {
                foreach (var caja in toExport)
                {
                    await _exportService.pdf2docs(caja.IdCarptera, hoja_control, ocr, sobreescribir);
                    Dispatcher.Invoke(() => exportPBar.Value++);
                }
            });

        }

        private void DlgRechazar_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            if (!string.IsNullOrEmpty(txtRechazo.Text))
            {
                int Registros = _formatoItems.Count();
                DialogResult result = System.Windows.Forms.MessageBox.Show($@"Seguro que desea devolver {Registros} Carpetas a Calidad?", "Enviar a Calidad", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    
                    foreach (CarpetaModel c in _formatoItems)
                    {
                        string cajaAnterior = c.nro_caja;
                        var Sql = "update t_carpeta set estado = 'I',idusr_control = NULL where id = " + c.IdCarptera;
                        Console.WriteLine(Sql);
                        var resultado = EntitiesRepository.Context.Database.ExecuteSqlCommand(Sql);
                        if (resultado == 1)
                        {
                            EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Historico estado
                            {
                                fase = "I",
                                id_carpeta = c.IdCarptera,
                                id_usuario = GlobalClass.id_usuario,
                                fecha_estado = DateTime.Now,
                                observacion = "Rechazo de Calidad por: " + txtRechazo.Text,
                                rechazado = 1
                            });
                            EntitiesRepository.Entities.SaveChanges();
                        }
                    }
                    if (Registros > 0) UpdateView();
                }
                else
                {
                    eventArgs.Cancel();
                    return;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Debe Especificar una razón para devolver el Lote");
                eventArgs.Cancel();
                return;
            }
        }

        private void btnDatosLote_Click(object sender, RoutedEventArgs e)
        {
            if (idcarpetaSeleccion != null && idcarpetaSeleccion != 0)
            {
                var carpeta = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Include("t_tercero").Include("t_lote").Include("t_carpeta_estado").Where(x => x.id == idcarpetaSeleccion)
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
                   tomo = x.tomo
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
                    System.Windows.MessageBox.Show("Error al mostrar la carpeta actual.", "Error carpeta", MessageBoxButton.OK);
                }
            }
        }
    }
}
