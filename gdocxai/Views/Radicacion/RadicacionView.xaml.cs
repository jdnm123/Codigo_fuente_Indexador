using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Gestion.DAL;
using MaterialDesignThemes.Wpf;

namespace Indexai.Views
{
    /// <summary>
    /// Lógica de interacción para RadicacionView.xaml
    /// </summary>
    public partial class RadicacionView : UserControl
    {

        public RadicacionView()
        {
            InitializeComponent();
            LoadData();

        }

        private void LoadData()
        {

            //Serie
            var listaSeries = new ObservableCollection<p_serie>(EntitiesRepository.Entities.p_serie).ToList();
            foreach (var serie in listaSeries)
            {
                selectSerie.Items.Add(serie.nombre);
            }

            //SubSerie
            var listaSubSeries = new ObservableCollection<p_subserie>(EntitiesRepository.Entities.p_subserie).ToList();
            foreach (var subSerie in listaSubSeries)
            {
                selectSubSerie.Items.Add(subSerie.nombre);
            }

            //Tipo Documental
            var listaTdocumental = new ObservableCollection<p_tipodoc>(EntitiesRepository.Entities.p_tipodoc).ToList();
            foreach (var tipoSocumental in listaTdocumental)
            {
                selectTDocumental.Items.Add(tipoSocumental.nombre);
            }

            //Dependencia
            var listaDependencia = new ObservableCollection<p_dependencia>(EntitiesRepository.Entities.p_dependencia).ToList();
            foreach (var dependencia in listaDependencia)
            {
                selectDependenciaResponsable.Items.Add(dependencia.nombre);
            }

            //Usuario Responsable
            var listaUsuarioResponsable = new ObservableCollection<p_usuario>(EntitiesRepository.Entities.p_usuario).ToList();
            foreach (var usuario in listaUsuarioResponsable)
            {

                string text = usuario.identificacion + " - " + usuario.nombres + " " + usuario.apellidos;
                int value = usuario.identificacion;

                this.selectUsuarioResponsable.SelectedValuePath = "Key";
                this.selectUsuarioResponsable.DisplayMemberPath = "Value";

                this.selectCopiarUsuarios.SelectedValuePath = "Key";
                this.selectCopiarUsuarios.DisplayMemberPath = "Value";

                selectUsuarioResponsable.Items.Add(new KeyValuePair<int, string>(value, text));
                selectCopiarUsuarios.Items.Add(new KeyValuePair<int, string>(value, text));

            }

            //Remitente
            var listaTerceros = new ObservableCollection<t_tercero>(EntitiesRepository.Entities.t_tercero).ToList();
            foreach (var tercero in listaTerceros)
            {
                this.selectUsuarioAsignado.SelectedValuePath = "Key";
                this.selectUsuarioAsignado.DisplayMemberPath = "Value";
                selectUsuarioAsignado.Items.Add(new KeyValuePair<string, string>(tercero.identificacion, tercero.nombres + " " + tercero.apellidos));

            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;
                // TextBlock1.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }

        private void btnRadicar_Click(object sender, RoutedEventArgs e)
        {
            /*
            selectAsuntosPredeterminados
            txtBoxDetalleAsunto
            selectMedioRecepcion
            txtRefDesAnexos
            textBoxReferenciaCliente
            calendarFechaVencimiento
            selectSerie
            selectTDocumental
            selectRegional
            selectSubSerie
            txtDiasTermino
            selectClasificacion
            selectDependenciaResponsable
            selectUsuarioResponsable
            */


            using (gdocxEntities context = new gdocxEntities())
            {
                var radicado = new tr_radicado()
                {
                    cod_radicado = "2324TQWD",
                    fecha = DateTime.Now,
                    id_tercero = 1,
                    direccion = "Calle Falsa 123",
                    telefono = "3465165659",
                    email = "email@Email.com",
                    lugar = "Medellín",
                    id_dependencia = 1,
                    asunto = "Notificación Salarial",
                    fecha_vencimiento = DateTime.Now,
                    id_medio = 1,
                    estado = 1,
                    id_tipo = 1,
                    valido = true,
                    id_usuario = 1
                };
                context.tr_radicado.Add(radicado);
                context.SaveChanges();
                MessageBox.Show("Radicado guardado satisfactoriamente");

            }

        }

        private void btnActualizarRemitente_Click(object sender, RoutedEventArgs e)
        {
            string idRemitente = ((KeyValuePair<string, string>)selectUsuarioAsignado.SelectedItem).Key;

            string telefono = txtTelefono.Text;
            string direccion = txtDireccion.Text;
            string tipoUsuario = selectTipoUsuario.Text;
            string email = txtEmail.Text;
            string departamento = selectDepartamento.Text;
            string ciudad = selectCiudad.Text;

            using (gdocxEntities context = new gdocxEntities())
            {
                var remitenteActualizado = context.t_tercero.Where(x => x.identificacion == idRemitente).FirstOrDefault();
                remitenteActualizado.telefono = telefono;
                remitenteActualizado.direccion = direccion;
                remitenteActualizado.email = email;
                remitenteActualizado.lugar = ciudad;
                remitenteActualizado.cargo = tipoUsuario;
                context.SaveChanges();
            }

        }

        private void selectUsuarioAsignado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string idRemitente = ((KeyValuePair<string, string>)selectUsuarioAsignado.SelectedItem).Key;
            var remitente = EntitiesRepository.Entities.t_tercero.Where(x => x.identificacion == idRemitente);
            var user = remitente.FirstOrDefault();
            string primeraLetra = user.nombres.Substring(1, 0);
            string segundaLetra = user.apellidos.Substring(1, 0);

            //Asignación básica de valores a ventana
            chipNombreUsuario.Content = user.nombres + " " + user.apellidos;
            chipNombreUsuario.Icon = primeraLetra + segundaLetra;
            chipTipoUsuario.Content = user.tipo_tercero;
            txtTelefono.Text = user.telefono;
            txtDireccion.Text = user.direccion;
            txtEmail.Text = user.email;
            selectTipoUsuario.Text = user.cargo;
            selectDepartamento.Text = "Cundinamarca";
            selectCiudad.Text = user.lugar;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNuevoGuardarRemitente_Click(object sender, RoutedEventArgs e)
        {
            /*
             txtInputRemNombre
             txtInputRemApellido
             selectTipoDocumento
             txtInputRemIdentificacion
             selectTipoTercero
             selectLugarExp
             txtInputRemTelefono
             txtInputRemDireccion
             txtInputRemEmail
             selectRemCargo
             */
            using (gdocxEntities context = new gdocxEntities())
            {
                var tercero = new t_tercero()
                {
                    nombres = txtInputRemNombre.Text,
                    apellidos = txtInputRemApellido.Text,
                    tipo_documento = selectTipoDocumento.Text,
                    identificacion = txtInputRemIdentificacion.Text,
                    tipo_tercero = selectTipoTercero.Text,
                    lugar_exp = selectLugarExp.Text,
                    telefono = txtInputRemTelefono.Text,
                    direccion = txtInputRemDireccion.Text,
                    email = txtInputRemEmail.Text,
                    cargo = selectRemCargo.Text
                };
                context.t_tercero.Add(tercero);
                context.SaveChanges();
                MessageBox.Show("Remitente gurdado satisfactoriamente");

            }
        }
    }
}
