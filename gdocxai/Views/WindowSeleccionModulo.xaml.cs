using Gestion.DAL;
using Indexai.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Indexai.Views
{
    /// <summary>
    /// Interaction logic for WindowSeleccionModulo.xaml
    /// </summary>
    public partial class WindowSeleccionModulo : Window
    {
        private int _numExiste;
        private ICollection<p_usuario_perfil> _perfiles;

        public WindowSeleccionModulo()
        {
            InitializeComponent();
            GridModulos.ItemsSource = GlobalClass.Modulos;
        }

        private void GridModulos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridModulos.SelectedItem != null)
            {
                switch (((t_modulo)GridModulos.SelectedItem).nombre)
                {
                    case "Indexador":
                        ModuloLauncher.ShowIndexador(_numExiste, _perfiles);
                        Close();
                        break;
                    case "Radicacion":
                        ModuloLauncher.ShowRadicador(_numExiste, _perfiles);
                        Close();
                        break;
                }
            }
        }

        internal void SetPefiles(int numExiste, ICollection<p_usuario_perfil> perfiles)
        {
            _numExiste = numExiste;
            _perfiles = perfiles;
        }
    }
}