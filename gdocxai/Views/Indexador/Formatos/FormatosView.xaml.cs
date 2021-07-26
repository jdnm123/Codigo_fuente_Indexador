using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Models;
using Indexai.Services;
using MaterialDesignThemes.Wpf;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Indexai.Views
{
    /// <summary>
    /// Interaction logic for FormatosView.xaml
    /// </summary>
    public partial class FormatosView : System.Windows.Controls.UserControl
    {
        private ObservableCollection<CarpetaModel> _formatoItems;
        private IQueryable<t_carpeta> _baseFilteredQuery;
        private PdfExportService _exportService;

        public FormatosView()
        {
            InitializeComponent();
            //if (GlobalClass.loc_admin == 1 || GlobalClass.loc_calidad == 1) {
                Loaded += FormatosView_Loaded;
                _exportService = new PdfExportService();
                buscadorView.SetFormatos(this);
            //}
            string dirEXE = Path.GetFullPath(".\\");
            if (File.Exists(dirEXE + "/logo_" + GlobalClass.id_proyecto + ".png"))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dirEXE + "/logo_" + GlobalClass.id_proyecto + ".png", UriKind.Absolute);
                bitmap.EndInit();
                ImgLogo.Source = bitmap;
            }
        }

        private async void FormatosView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTabFormato();
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

        public void UpdateView()
        {
            _formatoItems = LoadFormatoItems();
            txtGridTotalitems.Content = "Total Documentos: " + _formatoItems.Count.ToString();
            Dispatcher.Invoke(() => exportPager.Source = _formatoItems);
        }


        private async void ExportCaja(object sender, DialogClosingEventArgs eventArgs)
        {
            exportPBar.Value = 0;
            if (!Equals(eventArgs.Parameter, true)) return; //valida si el dialog confirmó exportar caja
            if (!string.IsNullOrEmpty(txtExportCodCarpeta.Text))
            {
                System.Windows.MessageBox.Show("Seleccione la carpeta para exportar los PDF's", "Exportar", MessageBoxButton.OK);
                FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    IEnumerable<CarpetaModel> toExport = _formatoItems.DistinctBy(m => new { m.nro_caja, m.t_lote.nom_lote }).OrderBy(x => x.nro_caja);
                    exportPBar.Maximum = toExport.Count();
                    List<string> exportedList = new List<string>();
                    p_formato mipFormato = EntitiesRepository.Entities.p_formato.AsNoTracking().Where(f => f.id_proyecto == GlobalClass.id_proyecto).FirstOrDefault();

                    foreach (var caja in toExport)
                    {
                        string text = txtExportCodCarpeta.Text;
                        await Task.Run(() => {

                            if (mipFormato.ver_rcj == 1)
                            {
                                _exportService.ExportPfdCaja(GlobalClass.GetNumber(text, 1),caja.t_lote.id, caja.nro_caja, openFolderDialog.SelectedPath, ref exportedList);
                            }
                            else if (mipFormato.ver_rcj == 2)
                            {
                                _exportService.ExportPfdCaja2(GlobalClass.GetNumber(text, 1), caja.t_lote.nom_lote, caja.nro_caja, openFolderDialog.SelectedPath, ref exportedList, mipFormato);
                            }
                            else if (mipFormato.ver_rcj == 3)
                            {
                                _exportService.ExportPfdCaja3(GlobalClass.GetNumber(text, 1), caja.t_lote.nom_lote, GlobalClass.GetNumber(caja.nro_caja), openFolderDialog.SelectedPath, ref exportedList, mipFormato);
                            }
                            
                        });
                        
                        exportPBar.Value++;
                    }
                    string joinName = $"{openFolderDialog.SelectedPath}" + "/Caja_" + exportedList.Count + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";


                    PdfDocument finalDoc = new PdfDocument();


                    // Merges PDFDocument.

                    PdfDocument.Merge(finalDoc, exportedList.ToArray());

                    //Saves the final document

                    finalDoc.Save(joinName);

                    //Closes the document

                    finalDoc.Close(true);

                    foreach (var fileToDelete in exportedList)
                    {
                        File.Delete(fileToDelete);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Se canceló la operación de exportación.");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Debe ingresar un código válido para exportar.");
            }
            txtExportCodCarpeta.Text = string.Empty;
        }


        private async void BtnExportarCarpeta_Click(object sender, RoutedEventArgs e)
        {
            exportPBar.Value = 0;

            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                IEnumerable<CarpetaModel> toExport = _formatoItems.DistinctBy(x => x.NoExpediente).OrderBy(x => x.NroCaja).ThenBy(x => x.nro_carpeta).ThenBy(x => x.NoExpediente);
                exportPBar.Maximum = toExport.Count();
                List<string> exportedList = new List<string>();
                p_formato mipFormato = EntitiesRepository.Entities.p_formato.AsNoTracking().Where(f => f.id_proyecto == GlobalClass.id_proyecto).FirstOrDefault();
                int consecutivo = 0;
                string cajactual = string.Empty;
                int carpetaActual = 0;
                foreach (var caja in toExport)
                {
                    await Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if(cajactual!=caja.nro_caja){
                                consecutivo = 0;
                                cajactual = caja.nro_caja;
                            }
                            consecutivo++;
                        });
                        if (mipFormato.ver_rkp == 1)
                        {
                            _exportService.ExportPdfRotuloCarpeta1(caja.IdCarptera, openFolderDialog.SelectedPath);
                        }
                        else if (mipFormato.ver_rkp == 2)
                        {
                            _exportService.ExportPdfRotuloCarpeta2(caja.IdCarptera, openFolderDialog.SelectedPath, ref exportedList);
                        }
                        else if (mipFormato.ver_rkp == 3)
                        {
                            _exportService.ExportPdfRotuloCarpeta3(caja.IdCarptera, openFolderDialog.SelectedPath, ref exportedList, mipFormato, consecutivo);
                        }
                        else if (mipFormato.ver_rkp == 4)
                        {
                            _exportService.ExportPdfRotuloCarpeta4(caja.IdCarptera, openFolderDialog.SelectedPath, ref exportedList, mipFormato);
                        }
                        else if (mipFormato.ver_rkp == 5)
                        {
                            if (carpetaActual != caja.nro_carpeta)
                            {
                                _exportService.ExportPdfRotuloCarpeta5(caja.IdCarptera, openFolderDialog.SelectedPath, ref exportedList, mipFormato);
                                carpetaActual = caja.nro_carpeta;
                            }
                            
                        }
                        else if (mipFormato.ver_rkp == 6)
                        {
                            if (carpetaActual != caja.nro_carpeta)
                            {
                                _exportService.ExportPdfRotuloCarpeta6(caja.IdCarptera, openFolderDialog.SelectedPath, ref exportedList, mipFormato);
                                carpetaActual = caja.nro_carpeta;
                            }

                        }
                    });
                   
                   //_exportService.ExportPdfRotuloCarpeta2(caja.IdCarptera, openFolderDialog.SelectedPath, ref exportedList));
                    exportPBar.Value++;
                }


                string joinName = $"{openFolderDialog.SelectedPath}" + "/RKP_" + exportedList.Count + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfDocument finalDoc = new PdfDocument();


                // Merges PDFDocument.

                PdfDocument.Merge(finalDoc, exportedList.ToArray());

                //Saves the final document

                finalDoc.Save(joinName);

                //Closes the document

                finalDoc.Close(true);

                foreach (var fileToDelete in exportedList)
                {
                    File.Delete(fileToDelete);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Se canceló la operación de exportación.");
            }
        }

        private async void BtnExportarHojaControl_Click(object sender, RoutedEventArgs e)
        {
            exportPBar.Value = 0;
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                //m => new { m.Caja, m.Lote.nom_lote }
                IEnumerable<CarpetaModel> toExport = _formatoItems.DistinctBy(m => new { m.NoExpediente, m.nom_expediente });
                exportPBar.Maximum = toExport.Count();
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("¿Desea adjuntar el PDF original a los documentos?", "Importante", MessageBoxButton.YesNo);

                p_formato mipFormato = EntitiesRepository.Entities.p_formato.AsNoTracking().Where(f => f.id_proyecto == GlobalClass.id_proyecto).FirstOrDefault();
                foreach (var caja in toExport)
                {
                    await Task.Run(() =>
                    {
                        if (mipFormato.ver_hc == 1)
                        {
                            _exportService.pdfHojaControl1(caja.IdCarptera, openFolderDialog.SelectedPath);
                        }
                        else if (mipFormato.ver_hc == 2)
                        {
                            _exportService.pdfHojaControl2_ok(caja.IdCarptera, openFolderDialog.SelectedPath, messageBoxResult == MessageBoxResult.Yes, mipFormato);
                        }
                        else if (mipFormato.ver_hc == 3)
                        {
                            _exportService.pdfHojaControl3(caja.IdCarptera, openFolderDialog.SelectedPath, messageBoxResult == MessageBoxResult.Yes, mipFormato);
                        }
                        else if (mipFormato.ver_hc == 4)
                        {
                            _exportService.pdfHojaControl4(caja.IdCarptera, openFolderDialog.SelectedPath, messageBoxResult == MessageBoxResult.Yes, mipFormato);
                        }
                    });
                    exportPBar.Value++;
                }

            }
            else
            {
                System.Windows.MessageBox.Show("Se canceló la operación de exportación.");
            }
        }

        private async void BtnExportarFuid_Click(object sender, RoutedEventArgs e)
        {
            exportPBar.Value = 0;
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                p_formato mipFormato = EntitiesRepository.Entities.p_formato.AsNoTracking().Where(f => f.id_proyecto == GlobalClass.id_proyecto).FirstOrDefault();
                IEnumerable<CarpetaModel> toExport = _formatoItems.DistinctBy(m => new { m.nro_caja, m.t_lote.nom_lote }).OrderBy(x => x.nro_caja);
                exportPBar.Maximum = toExport.Count();
                foreach (var caja in toExport)
                {
                    await Task.Run(() =>
                    {
                        if (mipFormato.ver_fi == 1)
                        {
                            _exportService.ExportPdfFuid1(exportPath: openFolderDialog.SelectedPath, caja.LoteId, nro_caja: caja.NroCaja.ToString());
                        }
                        else if (mipFormato.ver_fi == 2)
                        {
                            _exportService.ExportPdfFuid2(exportPath: openFolderDialog.SelectedPath, mipFormato, caja.t_lote.nom_lote, caja.NroCaja.ToString());
                        }
                        else if (mipFormato.ver_fi == 4)
                        {
                            _exportService.ExportPdfFuid4(exportPath: openFolderDialog.SelectedPath, mipFormato, caja.t_lote.nom_lote, caja.NroCaja);
                        }

                    });
                    exportPBar.Value++;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Se canceló la operación de exportación.");
            }
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
                    IdTercero = x.id_tercero,
                    NroCaja = (int)x.int_caja

                }).Take(3000).ToList()
            );
        }

        private IQueryable<t_carpeta> GetRootQuery()
        {
            var rootQuery = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.t_lote.id_proyecto == GlobalClass.id_proyecto && (x.estado == "I" || x.estado == "C")).AsQueryable();

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
        internal void SetFilter(string codCarpeta, string expediente, string nomLote, string numCaja)
        {
            _baseFilteredQuery = GetRootQuery();

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
        }

        private async void btnMaximoItemsGrid_Click(object sender, RoutedEventArgs e)
        {
           UpdateView();
        }

        private void btnExportarIndice_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public static class ExportExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
