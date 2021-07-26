using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Models;
using Indexai.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Indexai.Views
{
    /// <summary>
    /// Lógica de interacción para ProcesoOcrPdfa.xaml
    /// </summary>
    public partial class ProcesoFirma : UserControl
    {
        private ObservableCollection<CarpetaModel> _formatoItems;
        private IQueryable<t_carpeta> _baseFilteredQuery;
        private PdfExportService _exportService;

        public ProcesoFirma()
        {
            InitializeComponent();
            Loaded += FormatosView_Loaded;
            _exportService = new PdfExportService();
            //buscadorView.SetCargaView(this);
            if (GlobalClass.loc_admin == 1) CargaOpciones();
        }

        private void CargaOpciones()
        {

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
            txtGridTotalitems.Content = "Total registros: " + _formatoItems.Count().ToString("###.###.###");
        }

        internal async Task UpdateView()
        {
            _formatoItems = LoadFormatoItems();
            Dispatcher.Invoke(() => exportPager.Source = _formatoItems);
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

        private async void btnMaximoItemsGrid_Click(object sender, RoutedEventArgs e)
        {
            await UpdateView();
        }

        private IQueryable<t_carpeta> GetRootQuery()
        {
            var rootQuery = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.estado == "C" && x.t_lote.id_proyecto == GlobalClass.id_proyecto && x.exp_ocr == true && x.exp_pdfa == true && x.exp_firma == null).AsQueryable();

            /*rootQuery = buscadorView.GetQueryFilter(rootQuery);

            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(txtItemCount.Text))
                {
                    rootQuery = rootQuery.Take(Convert.ToInt32(txtItemCount.Text));
                }
            });*/
            return rootQuery;
        }

        private void btnProcesarFirma_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}