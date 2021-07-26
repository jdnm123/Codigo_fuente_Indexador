using Gestion.DAL;
using Gestion.DAL.Models;
using Indexai.Models;
using Indexai.Services;
using Syncfusion.Data;
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
    /// Lógica de interacción para AsignarCargaView.xaml
    /// </summary>
    public partial class AsignarCargaView : UserControl
    {
        private ObservableCollection<CarpetaModel> _formatoItems;
        private List<t_carpeta> _formatoItemsTOTAL;
        private IQueryable<t_carpeta> _baseFilteredQuery;
        private PdfExportService _exportService;

        public AsignarCargaView()
        {
            InitializeComponent();
            Loaded += FormatosView_Loaded;
            _exportService = new PdfExportService();
            buscadorView.SetCargaView(this);
            if (GlobalClass.loc_admin == 1) CargaUsuarios();
            exportPager.OnDemandLoading += ExportPager_OnDemandLoading;
        }

        private void ExportPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            int startIndex = e.StartIndex;
            UpdatePage(startIndex);
        }

        private void UpdatePage(int startIndex)
        {
            try
            {
                var query = LoadFormatoItems();

                _formatoItems = new ObservableCollection<CarpetaModel>(query.Skip(startIndex).Take(exportPager.PageSize).ToList());
                var rootQuery = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.t_lote.id_proyecto == GlobalClass.id_proyecto );
                _formatoItemsTOTAL = buscadorView.GetQueryFilter(rootQuery).Take(4000).ToList();
                int itemsCount = _formatoItemsTOTAL.Count();
                txtGridTotal.Content = "Total registros: " + itemsCount.ToString("###.###.###");

                //int itemsCount = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Where(x => x.estado == "I" && x.t_lote.id_proyecto == GlobalClass.id_proyecto && x.idusr_control == null).Count();
                int pageCount = (int)Math.Ceiling((double)itemsCount / exportPager.PageSize);
                exportPager.PageCount = pageCount != 0 ? pageCount : 1;
                if (_formatoItems?.Count == 0)
                {
                    exportPager.PageCount = 0;
                }

                exportPager.LoadDynamicItems(startIndex, _formatoItems);
                if (exportPager.PageIndex != -1)
                    exportPager.PagedSource?.ResetCacheForPage(exportPager.PageIndex);
            }
            catch (Exception)
            {
            }
        }

        private void CargaUsuarios()
        {
            var queryUsr = from p in EntitiesRepository.Entities.p_usuario_perfil
                           join u in EntitiesRepository.Entities.p_usuario on p.id_usuario equals u.id
                           where p.loc_calidad == 1 && p.id_proyecto == GlobalClass.id_proyecto
                           orderby u.usuario
                           select new { Nombre = u.usuario, Id = u.id };
            cbx_Usrcontrol.ItemsSource = queryUsr.ToList();
            cbx_Usrcontrol.DisplayMemberPath = "Nombre";
            cbx_Usrcontrol.DefaultText = "Seleccione Usuarios";
        }

        private void FormatosView_Loaded(object sender, RoutedEventArgs e)
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
            //exportPager.Source = _formatoItems;
        }

        public async Task UpdateView()
        {
            UpdateWithReset();
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
                    Indexado = x.p_usuario.usuario ?? "NA",
                    Estado = x.estado,
                    IdUsuario = x.idusr_asignado,
                    NoExpediente = x.nro_expediente.ToString(),
                    IdSubSerie = x.t_lote.id_subserie ?? -1,
                    IdCarptera = x.id,
                    IdTercero = x.id_tercero
                })
            );
        }

        private void btnMaximoItemsGrid_Click(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private IQueryable<t_carpeta> GetRootQuery()
        {
            IQueryable<t_carpeta> rootQuery;
            if (chkMostrarTodo.IsChecked.Value)
            {
                var filterDateTime = DateTime.Now.AddDays(-0.5);
                rootQuery = EntitiesRepository.Entities.t_carpeta.AsNoTracking()
                                                   .Where(x => x.t_lote.id_proyecto == GlobalClass.id_proyecto).AsQueryable();
            }
            else
            {
                rootQuery = EntitiesRepository.Entities.t_carpeta.AsNoTracking()
                    .Where(x => x.estado == "I" &&
                    x.t_lote.id_proyecto == GlobalClass.id_proyecto).AsQueryable();
            }

            rootQuery = buscadorView.GetQueryFilter(rootQuery);
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(txtItemCount.Text))
                {
                    rootQuery = rootQuery.Take(Convert.ToInt32(txtItemCount.Text));
                }else rootQuery = rootQuery.Take(4000);
            });
            return rootQuery;
        }

        private async void btnAsignarPorciento_Click(object sender, RoutedEventArgs e)
        {
            if (chkMostrarTodo.IsChecked.Value)
            {
                MessageBox.Show("No puede asignar cuando este habilitado Mostrar todo");
                return;
            }
            int NumberOfRows = exportGrid.View.Records.Count();
            if (NumberOfRows == 0)
            {
                MessageBox.Show("No hay expedientes indexados para asignar");
                return;
            }
            if (cbx_Usrcontrol.SelectedItems == null)
            {
                MessageBox.Show("Debe seleccionar al menos un usuario");
                return;
            }
            int porcentaje = GlobalClass.GetNumber(txtPorcientoAsignar.Text);
            if (porcentaje == 0 || porcentaje < 1 || porcentaje > 100)
            {
                MessageBox.Show("Debe seleccionar un Porcentaje a asignar entre (1-100)");
                return;
            }
            List<t_carpeta> _formatoItemsAsignar = _formatoItemsTOTAL.Where(o => o.estado == "I").ToList();
            int totalFolios = 0, numRegPorcentaje = 0, TotalRegistros = _formatoItemsAsignar.Count();
            numRegPorcentaje = GlobalClass.GetNumber((TotalRegistros * porcentaje / 100).ToString());
            if (numRegPorcentaje == 0)
            {
                MessageBox.Show($"No se pueden asignar el {porcentaje} % a {TotalRegistros} registros");
                return;
            }
            ////////CUENTA LOS USUARIOS ASIGNADOS
            List<int> listIdUsuario = new List<int>();
            foreach (var itemU in cbx_Usrcontrol.SelectedItems)
            {
                System.Reflection.PropertyInfo pi = itemU.GetType().GetProperty("Id");
                listIdUsuario.Add((int)(pi.GetValue(itemU, null)));
            }
            /************* OBTIENE ALEATOREAMENTE LAS CARPETAS A REVISAR **********/
            Random rand = new Random();
            List<int> listNumRamAsignados = new List<int>();
            int number;
            for (int i = 0; i < numRegPorcentaje; i++)
            {
                do
                {
                    number = rand.Next(1, TotalRegistros + 1);
                } while (listNumRamAsignados.Contains(number));
                listNumRamAsignados.Add(number);
            }
            /**************** Suma los folios del porcentaje anteriormente asignado ****************/
            for (int i = 0; i < listNumRamAsignados.Count; i++)
            {
                totalFolios += GlobalClass.GetNumber(_formatoItemsAsignar[listNumRamAsignados[i] - 1].total_folios.ToString());
            }
            /********** Realiza asignación se Trabajo **********/
            int posusuario = 0;
            float promedioFolios = totalFolios / numRegPorcentaje;
            float cambio = totalFolios / listIdUsuario.Count();
            float tmpSumFolios = 0;
            DateTime fechaActual = DateTime.Now;
            for (int i = 0; i < listNumRamAsignados.Count; i++)
            {
                if (listIdUsuario[posusuario] != 0) //PROBLEMA DE INDEX AQUÍ (CORREGIDO)
                {
                    var result = EntitiesRepository.Entities.t_carpeta.Find(_formatoItemsAsignar[listNumRamAsignados[i] - 1].id);  //db.Books.SingleOrDefault(b => b.BookNumber == bookNumber);
                    if (result != null)
                    {
                        //result.idusr_control = listIdUsuario[posusuario]; //PROBLEMA DE INDEX AQUÍ
                        var Sql = "update t_carpeta set idusr_control = '" + listIdUsuario[posusuario] + "' where id = " + _formatoItemsAsignar[listNumRamAsignados[i] - 1].id;
                        await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync(Sql);
                        //await EntitiesRepository.Entities.SaveChangesAsync();
                        //Guarda registro de Estado
                        EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Historico Carpeta
                        {
                            id_carpeta = _formatoItemsAsignar[listNumRamAsignados[i] - 1].id,
                            id_usuario = GlobalClass.id_usuario,
                            fase = "I",
                            fecha_estado = fechaActual,
                            observacion = $@"Control calidad Asignado en {porcentaje}%",
                        });
                    }
                }
                tmpSumFolios += GlobalClass.GetNumber(_formatoItemsAsignar[listNumRamAsignados[i] - 1].total_folios.ToString());
                //Si no es el último reivsa si debe cambiar de usuario
                if (posusuario + 1 < listIdUsuario.Count)
                {
                    if (tmpSumFolios > (cambio * (1 + posusuario))) posusuario++;
                }
            }
            /***************** LOS QUE NO FUERON ASIGNADOS CLASIFICARLOS CON ESTADO C COMO SI YA ESTUVIERAN REVISADOS *****************/
            for (int i = 0; i < TotalRegistros; i++)
            {
                if (!listNumRamAsignados.Contains((i + 1)))
                {
                    var resultNA = EntitiesRepository.Entities.t_carpeta.Find(_formatoItemsAsignar[i].id);  //db.Books.SingleOrDefault(b => b.BookNumber == bookNumber);
                    if (resultNA != null)
                    {
                        //result.idusr_control = GlobalClass.id_usuario;
                        //result.estado = "C";
                        //await EntitiesRepository.Entities.SaveChangesAsync(); REVISAR SI SE DEBE ACTIVAR
                        var Sql = "update t_carpeta set estado = 'C', idusr_control = '" + listIdUsuario[posusuario] + "' where id = " + _formatoItemsAsignar[i].id;
                        await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync(Sql);
                        //Guarda registro de Estado
                        EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Historico Carpeta
                        {
                            id_carpeta = _formatoItemsAsignar[i].id,
                            id_usuario = GlobalClass.id_usuario,
                            fase = "C",
                            fecha_estado = fechaActual,
                            observacion = $@"Control calidad Asignado en {100 - porcentaje}%",
                        });
                    }
                }
            }
            int changes = await EntitiesRepository.Entities.SaveChangesAsync();

            MessageBox.Show("Asignación exitosa de " + listNumRamAsignados.Count + " registros");
            UpdateWithReset();
        }

        /// <summary>
        /// Actualiza la página forzando el pager a recargar los datos del cache.
        /// </summary>
        internal void UpdateWithReset()
        {
            btnActualizar.IsEnabled = false;
            try
            {
                if (exportPager.PagedSource != null) //verifica que el pager contiene una lista
                {
                    exportPager.PagedSource.ResetCache();
                    if (exportPager.PageIndex != -1)
                        exportPager.PagedSource.ResetCacheForPage(exportPager.PageIndex);
                    exportPager.PagedSource.MoveToPage(exportPager.PageIndex);
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }

            btnActualizar.IsEnabled = true;
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            UpdateWithReset();
        }

        private void chkMostrarTodo_Checked(object sender, RoutedEventArgs e)
        {
            UpdateWithReset();
        }

        private async void btnCambiarUsuario_Click(object sender, RoutedEventArgs e)
        {
            List<int> listIdUsuario = new List<int>();
            foreach (var itemU in cbx_Usrcontrol.SelectedItems)
            {
                System.Reflection.PropertyInfo pi = itemU.GetType().GetProperty("Id");
                listIdUsuario.Add((int)(pi.GetValue(itemU, null)));
            }

            if (listIdUsuario.Count == 1)
            {
                btnCambiarUsuario.IsEnabled = false;
                var idUsuario = listIdUsuario.FirstOrDefault();

                foreach (var carpeta in _formatoItemsTOTAL)
                {
                    if (carpeta.estado == "D" || carpeta.estado == "I")
                    {
                        //result.idusr_control = listIdUsuario[posusuario]; //PROBLEMA DE INDEX AQUÍ
                        var Sql = "update t_carpeta set id_usuario = '" + idUsuario + "', idusr_control = '" + idUsuario + "' where id = " + carpeta.id;
                        if (carpeta.estado == "D") Sql = "update t_carpeta set id_usuario = '" + idUsuario + "', idusr_asignado = '" + idUsuario + "' where id = " + carpeta.id;
                        await EntitiesRepository.Context.Database.ExecuteSqlCommandAsync(Sql);
                        //Guarda registro de Estado
                        EntitiesRepository.Entities.t_carpeta_estado.Add(new t_carpeta_estado //Historico Carpeta
                        {
                            id_carpeta = carpeta.id,
                            id_usuario = GlobalClass.id_usuario,
                            fase = carpeta.estado,
                            fecha_estado = DateTime.Now,
                            observacion = $@"Cambio de ususario asignado.",
                        });
                        await EntitiesRepository.Context.SaveChangesAsync();
                    }
                }

                UpdateWithReset();
                btnCambiarUsuario.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Se requiere seleccionar solamente 1 usuario.",
                    "Requerimiento selección", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            
        }
    }
}