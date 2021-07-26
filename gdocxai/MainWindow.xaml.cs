using Dapper;
using Gestion.DAL;
using Indexai.Helpers;
using Indexai.Services;
using Indexai.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Indexai
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
            lblError.Visibility = Visibility.Hidden;
            GlobalClass.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblVersion.Content = "Versión: " + GlobalClass.version;
            pasClave.KeyUp += PasClave_KeyUp;
            /*DEPURACIÓN
            txtUsuario.Text = "ad123";
            pasClave.Password = "123";
            Login();*/
            WarmUpSQL();
        }

        /// <summary>
        /// Inicia y cierra una conexión SQL.
        /// </summary>
        private void WarmUpSQL()
        {
            using (SqlConnection conn =
                                 new SqlConnection(EntitiesRepository.CS))
            {
                conn.Open();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void PasClave_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        public async void Login()
        {
            loadingAnimation.Visibility = Visibility.Visible;
            var usr = txtUsuario.Text;
            if (string.IsNullOrEmpty(usr))
            {
                lblError.Content = "Debe ingresar usuario";
                lblError.Visibility = Visibility.Visible;
                txtUsuario.Focus();
            }
            else if (string.IsNullOrEmpty(pasClave.Password))
            {
                lblError.Content = "Debe ingresar contraseña";
                lblError.Visibility = Visibility.Visible;
                lblError.Focus();
            }
            else
            {
                btnIngresar.IsEnabled = false;
                lblError.Content = "Espere por favor ...";
                lblError.Visibility = Visibility.Visible;
                var cla = Base64Encode(Base64Encode(Base64Encode(pasClave.Password)));

                lblError.Visibility = Visibility.Hidden;

                Stopwatch w = new Stopwatch();
                var usuario = EntitiesRepository.LoadUser(usr, cla);


                if (usuario == null)
                {
                    lblError.Content = "No se encontró acceso";
                    lblError.Visibility = Visibility.Visible;
                    loadingAnimation.Visibility = Visibility.Collapsed;
                    btnIngresar.IsEnabled = true;
                    return;
                }

                var perfiles = usuario.p_usuario_perfil;
                var numExiste = perfiles.Count();

                GlobalClass.id_usuario = usuario.id;
                GlobalClass.nom_usuario = usr;
                GlobalClass.nombres = usuario.nombres;
                GlobalClass.apellidos = usuario.apellidos;
                GlobalClass.clave = cla;
                GlobalClass.email = usuario.email;


                if (usuario.t_modulo.Count == 0) //por defecto inicia en el indexador si no hay módulos registrados para el usuario
                {
                    ShowIndexador(numExiste, perfiles);
                }
                else
                {
                    if (usuario.t_modulo.Count == 1)
                    {
                        switch (usuario.t_modulo.FirstOrDefault().nombre)
                        {
                            case "Indexador":
                                ShowIndexador(numExiste, perfiles);
                                break;
                            case "Radicacion":
                                throw new NotImplementedException();
                                break;
                            default:
                                ShowIndexador(numExiste, perfiles);
                                break;
                        }
                    }
                    else
                    {
                        GlobalClass.Modulos = usuario.t_modulo;
                        Dispatcher.Invoke(() =>
                        {
                            WindowSeleccionModulo seleccionModulo = new WindowSeleccionModulo();
                            seleccionModulo.SetPefiles(numExiste, perfiles);
                            seleccionModulo.Show();
                        });
                    }
                }

                Dispatcher.Invoke(() => Close());

            }
        }

        

        private void ShowIndexador(int numExiste, ICollection<p_usuario_perfil> perfiles)
        {
            if (numExiste == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    lblError.Content = "No tiene permisos";
                    lblError.Visibility = Visibility.Visible;
                    loadingAnimation.Visibility = Visibility.Collapsed;
                    btnIngresar.IsEnabled = true;
                });
                return;
            }
            else
            {
                ModuloLauncher.ShowIndexador(numExiste, perfiles);
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void BtnMinimiza_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}