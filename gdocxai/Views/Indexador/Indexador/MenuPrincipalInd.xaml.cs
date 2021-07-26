using Indexai.Services;
using Indexai.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Indexai
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipalInd.xaml
    /// </summary>
    ///
    public partial class MenuPrincipalInd : Window
    {
        private static readonly Dictionary<int, UserControl> views = new Dictionary<int, UserControl>();
        private int _idCarpeta;

        public MenuPrincipalInd()
        {
            InitializeComponent();
            Loaded += MenuPrincipal_Loaded;
            Closing += MenuPrincipal_Closing;
            //maintabs.SelectedIndexChanged += Maintabs_SelectedIndexChanged;
            string dirAPP = AppDomain.CurrentDomain.BaseDirectory;
            Telemetry.TrackEvent($"App iniciada en : {dirAPP}, Usuario {GlobalClass.nom_usuario}");
            GlobalClass.ViewController = this;
        }

        /// <summary>
        /// Envía la carpta a control de calidad para edición.
        /// </summary>
        /// <param name="idCarpeta">Id de la carpeta</param>
        public void EnviarCarpetaCalidad(int idCarpeta)
        {
            DemoItemsListBox.SelectedIndex = 1;
            (views[DemoItemsListBox.SelectedIndex] as ControlCalidadView).Loaded += MenuPrincipalInd_Loaded;
            _idCarpeta = idCarpeta;
        }

        private void MenuPrincipalInd_Loaded(object sender, RoutedEventArgs e)
        {
            (views[DemoItemsListBox.SelectedIndex] as ControlCalidadView).Loaded -= MenuPrincipalInd_Loaded;
            (views[DemoItemsListBox.SelectedIndex] as ControlCalidadView).SetCarpeta(_idCarpeta, true);
            (views[DemoItemsListBox.SelectedIndex] as ControlCalidadView).ShowPdfViewer();
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

        private void DemoItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            switch (DemoItemsListBox.SelectedIndex)
            {
                case 0:
                    if (Container != null)
                    {
                        if (views.ContainsKey(DemoItemsListBox.SelectedIndex))
                        {
                            SetSize(views[DemoItemsListBox.SelectedIndex]);
                            Container.Content = views[DemoItemsListBox.SelectedIndex];
                            (views[DemoItemsListBox.SelectedIndex] as IndexadorView).UpdateWithReset();
                            if (GlobalClass.FromIndexado)
                            {
                                (views[DemoItemsListBox.SelectedIndex] as IndexadorView).SetPublicIndex();
                                GlobalClass.FromIndexado = false;
                            }
                        }
                        else
                        {
                            IndexadorView view = new IndexadorView();
                            SetSize(view);
                            views.Add(DemoItemsListBox.SelectedIndex, view);
                            Container.Content = view;
                        }
                    }
                    break;
                case 1:
                    if (Container != null)
                    {
                        if (views.ContainsKey(DemoItemsListBox.SelectedIndex))
                        {
                            SetSize(views[DemoItemsListBox.SelectedIndex]);
                            Container.Content = views[DemoItemsListBox.SelectedIndex];
                            (views[DemoItemsListBox.SelectedIndex] as ControlCalidadView).UpdateWithReset();
                        }
                        else
                        {
                            ControlCalidadView view = new ControlCalidadView();
                            SetSize(view);
                            views.Add(DemoItemsListBox.SelectedIndex, view);
                            Container.Content = view;
                        }
                    }
                    break;
                case 2:
                    if (Container != null)
                    {
                        if (views.ContainsKey(DemoItemsListBox.SelectedIndex))
                        {
                            SetSize(views[DemoItemsListBox.SelectedIndex]);
                            Container.Content = views[DemoItemsListBox.SelectedIndex];
                        }
                        else
                        {
                            FormatosView view = new FormatosView();
                            SetSize(view);
                            views.Add(DemoItemsListBox.SelectedIndex, view);
                            Container.Content = view;
                        }
                    }
                    break;
                case 3:
                    if (Container != null)
                    {
                        if (views.ContainsKey(DemoItemsListBox.SelectedIndex))
                        {
                            SetSize(views[DemoItemsListBox.SelectedIndex]);
                            Container.Content = views[DemoItemsListBox.SelectedIndex];
                        }
                        else
                        {
                            BusquedaVozView view = new BusquedaVozView();
                            SetSize(view);
                            views.Add(DemoItemsListBox.SelectedIndex, view);
                            Container.Content = view;
                        }
                    }
                    break;
                case 4:
                    if (Container != null)
                    {
                        if (views.ContainsKey(DemoItemsListBox.SelectedIndex))
                        {
                            SetSize(views[DemoItemsListBox.SelectedIndex]);
                            Container.Content = views[DemoItemsListBox.SelectedIndex];
                        }
                        else
                        {
                            AdministradorView view = new AdministradorView();
                            SetSize(view);
                            views.Add(DemoItemsListBox.SelectedIndex, view);
                            Container.Content = view;
                        }
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
                tabControlCalidad.Visibility = Visibility.Visible;
                tabFormatos.Visibility = Visibility.Visible;
                tabBusqueda.Visibility = Visibility.Visible;
                tabAdmin.Visibility = Visibility.Visible;
            }
            if (GlobalClass.loc_calidad == 1)
            {
                tabControlCalidad.Visibility = Visibility.Visible;
                tabFormatos.Visibility = Visibility.Visible;
            }
            if (GlobalClass.loc_consulta == 1) tabBusqueda.Visibility = Visibility.Visible;
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