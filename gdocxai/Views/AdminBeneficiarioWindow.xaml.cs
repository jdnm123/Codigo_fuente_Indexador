using Gestion.DAL;
using System;
using System.Windows;
using System.Windows.Input;

namespace Indexai
{
    /// <summary>
    /// Lógica de interacción para AdminBeneficiarioWindow.xaml
    /// </summary>
    public partial class AdminBeneficiarioWindow : Window
    { 
        public AdminBeneficiarioWindow()
        {
            InitializeComponent();
            Loaded += AdminBeneficiarioWindow_Loaded;
        }

        private void AdminBeneficiarioWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewBeneficiarios.SetParent(this);
        }

        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        internal void SetDocument(t_documento documento)
        {
            viewBeneficiarios.SetDcumento(documento);
        }
    }
}
