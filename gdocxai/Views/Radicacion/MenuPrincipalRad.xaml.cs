using Indexai.Services;
using Indexai.Views;
using Indexai.Views.Radicacion;
using Login2;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Indexai
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipalInd.xaml
    /// </summary>
    ///
    public partial class MenuPrincipalRad : Window
    {
        public MenuPrincipalRad()
        {
            InitializeComponent();
            Loaded += MenuPrincipal_Loaded;
            Closing += MenuPrincipal_Closing;
            //maintabs.SelectedIndexChanged += Maintabs_SelectedIndexChanged;
            string dirAPP = AppDomain.CurrentDomain.BaseDirectory;
            Telemetry.TrackEvent($"App iniciada en : {dirAPP}, Usuario {GlobalClass.nom_usuario}");
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void MenuLateralListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            switch (MenuLateralListBox.SelectedIndex)
            {
                case 0:
                    if (Container != null)
                    {
                        BandejaEntrada view = new BandejaEntrada();
                        SetSize(view);
                        Container.Content = view;
                    }
                    break;
                case 1:
                    if (Container != null)
                    {
                        RadicacionView view = new RadicacionView();
                        SetSize(view);
                        Container.Content = view;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Inicia el control con el tamaño del container
        /// </summary>
        /// <param name="view">Nuevo view</param>
        private void SetSize(UserControl view)
        {
            view.Height = Container.Height;
            view.Width = Container.Width;
        }

        private void Maintabs_SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //vIndexador?.ReleaseImageStream();
            //controlCalidadV?.ReleaseImages();
            //if (maintabs.SelectedIndex != 0)
            //{
            //    vIndexador.DisableIndex();
            //    vIndexador.CancelIndex();
            //}
            //if (maintabs.SelectedIndex == 0)
            //{
            //    vIndexador.SetCurrentNull();
            //}
            //if (maintabs.SelectedIndex == 1)
            //{
            //    controlCalidadV?.UpdateWithReset();
            //}


            //if (maintabs.SelectedIndex != 1)
            //{
            //    controlCalidadV.CancelControlCalidad();
            //}
        }

        private void MenuPrincipal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //vIndexador.Close();
            Environment.Exit(0);
        }

        private void MenuPrincipal_Loaded(object sender, RoutedEventArgs e)
        {
            Telemetry.SetUser(GlobalClass.nombres + " " + GlobalClass.apellidos);
            txtUsuario.Text = GlobalClass.nombres + " " + GlobalClass.apellidos;
            lblVersion.Content = "Versión: " + GlobalClass.version;
            if (GlobalClass.loc_admin == 1)
            {
                tabRadicacion.Visibility = Visibility.Visible;
            }
            if (GlobalClass.loc_calidad == 1)
            {
               // tabControlCalidad.Visibility = Visibility.Visible;
                //tabFormatos.Visibility = Visibility.Visible;
            }
            //if (GlobalClass.loc_consulta == 1) tabBusqueda.Visibility = Visibility.Visible;
        }

        private void BtnCerrarsesion_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal void UpdateIndexGrids()
        {
            //vIndexador.UpdateWithReset();
        }

        private void Container_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UserControl content = ((UserControl)Container.Content);
            if (content != null)
            {
                content.Width = e.NewSize.Width;
                content.Height = e.NewSize.Height;
            }
        }
    }
}