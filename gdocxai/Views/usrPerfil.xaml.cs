using Gestion.DAL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Indexai
{
    /// <summary>
    /// Lógica de interacción para usrPerfil.xaml
    /// </summary>
    public partial class usrPerfil : Window
    {
        public usrPerfil()
        {
            InitializeComponent();
            dgPerfil.ItemsSource = GlobalClass.PerfiList;
        }

        private void dgPerfil_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        gdperfil seleccion = (gdperfil)dgr.Item;
                        GlobalClass.id_proyecto = seleccion.id_proyecto;
                        GlobalClass.nom_proyecto = seleccion.nom_proyecto;
                        GlobalClass.ruta_proyecto = seleccion.ruta_proyecto;
                        GlobalClass.ruta_salida = seleccion.ruta_salida;
                        GlobalClass.estructura_export = seleccion.estructura_export;
                        GlobalClass.nombre_export = seleccion.nombre_export;
                        GlobalClass.superadmin = seleccion.superadmin;
                        GlobalClass.loc_admin = seleccion.loc_admin;
                        GlobalClass.loc_index = seleccion.loc_index;
                        GlobalClass.loc_calidad = seleccion.loc_calidad;
                        GlobalClass.loc_consulta = seleccion.loc_consulta;
                        MenuPrincipalInd winMenu = new MenuPrincipalInd();
                        winMenu.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void usrPerfil_Loaded(object sender, RoutedEventArgs e)
        {
            dgPerfil.Columns[2].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[3].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[4].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[5].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[6].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[7].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[8].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[9].Visibility = Visibility.Collapsed;
            dgPerfil.Columns[10].Visibility = Visibility.Collapsed;
        }
    }
}