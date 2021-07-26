using Dapper;
using Gestion.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace Indexai.Helpers
{
    public static class ModuloLauncher
    {
        public static void ShowIndexador(int numExiste, ICollection<p_usuario_perfil> perfiles)
        {
            p_proyecto dProyecto = null;

            if (numExiste == 1)
            {
                var usrDetalle = perfiles.FirstOrDefault();

                string tSQl = @"SELECT * FROM p_proyecto WHERE
                         id = @idProyecto ";
                
                using (var conn = new System.Data.SqlClient.SqlConnection(EntitiesRepository.CS))
                {
                    dProyecto = conn.Query<p_proyecto>(tSQl, new { idProyecto = usrDetalle.id_proyecto }).FirstOrDefault();
                }

                GlobalClass.id_proyecto = usrDetalle.id_proyecto;
                GlobalClass.nom_proyecto = dProyecto.nom_proyecto;
                GlobalClass.ruta_proyecto = dProyecto.ruta_proyecto;
                GlobalClass.ruta_salida = dProyecto.ruta_salida;
                GlobalClass.estructura_export = dProyecto.estructura_export;
                GlobalClass.nombre_export = dProyecto.nombre_export;
                GlobalClass.SortColumns = dProyecto.ordernar_calidad;
                GlobalClass.superadmin = usrDetalle.superadmin;
                GlobalClass.loc_admin = usrDetalle.loc_admin;
                GlobalClass.loc_index = usrDetalle.loc_index;
                GlobalClass.loc_calidad = usrDetalle.loc_calidad;
                GlobalClass.loc_consulta = usrDetalle.loc_consulta;
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    MenuPrincipalInd winMenu = new MenuPrincipalInd();
                    winMenu.Show();
                });

            }
            else
            {
                List<gdperfil> PerfiList = new List<gdperfil>();
                foreach (var item in perfiles)
                {
                    string tSQl = @"SELECT * FROM p_proyecto WHERE
                         id = @idProyecto ";
                    using (var conn = new System.Data.SqlClient.SqlConnection(EntitiesRepository.CS))
                    {
                        dProyecto = conn.Query<p_proyecto>(tSQl, new { idProyecto = item.id_proyecto }).FirstOrDefault();
                    }

                    gdperfil pItem = new gdperfil
                    {
                        id_proyecto = item.id_proyecto,
                        nom_proyecto = dProyecto.nom_proyecto,
                        ruta_proyecto = dProyecto.ruta_proyecto,
                        ruta_salida = dProyecto.ruta_salida,
                        estructura_export = dProyecto.estructura_export,
                        nombre_export = dProyecto.nombre_export,
                        superadmin = item.superadmin,
                        loc_admin = item.loc_admin,
                        loc_index = item.loc_index,
                        loc_calidad = item.loc_calidad,
                        loc_consulta = item.loc_consulta,
                    };
                    PerfiList.Add(pItem);
                }
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    GlobalClass.PerfiList = PerfiList;
                    usrPerfil winPerfil = new usrPerfil();
                    winPerfil.Show();
                });
            }
        }

        public static void ShowRadicador(int numExiste, ICollection<p_usuario_perfil> perfiles)
        {
            if (numExiste == 1)
            {
                var usrDetalle = perfiles.FirstOrDefault();
                GlobalClass.id_proyecto = usrDetalle.id_proyecto;
                GlobalClass.nom_proyecto = usrDetalle.p_proyecto.nom_proyecto;
                GlobalClass.ruta_proyecto = usrDetalle.p_proyecto.ruta_proyecto;
                GlobalClass.ruta_salida = usrDetalle.p_proyecto.ruta_salida;
                GlobalClass.estructura_export = usrDetalle.p_proyecto.estructura_export;
                GlobalClass.nombre_export = usrDetalle.p_proyecto.nombre_export;
                GlobalClass.superadmin = usrDetalle.superadmin;
                GlobalClass.loc_admin = usrDetalle.loc_admin;
                GlobalClass.loc_index = usrDetalle.loc_index;
                GlobalClass.loc_calidad = usrDetalle.loc_calidad;
                GlobalClass.loc_consulta = usrDetalle.loc_consulta;
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    MenuPrincipalRad winMenu = new MenuPrincipalRad();
                    winMenu.Show();
                });

            }
            else
            {
                List<gdperfil> PerfiList = new List<gdperfil>();
                foreach (var item in perfiles)
                {
                    gdperfil pItem = new gdperfil
                    {
                        id_proyecto = item.id_proyecto,
                        nom_proyecto = item.p_proyecto.nom_proyecto,
                        ruta_proyecto = item.p_proyecto.ruta_proyecto,
                        ruta_salida = item.p_proyecto.ruta_salida,
                        estructura_export = item.p_proyecto.estructura_export,
                        nombre_export = item.p_proyecto.nombre_export,
                        superadmin = item.superadmin,
                        loc_admin = item.loc_admin,
                        loc_index = item.loc_index,
                        loc_calidad = item.loc_calidad,
                        loc_consulta = item.loc_consulta,
                    };
                    PerfiList.Add(pItem);
                }
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    GlobalClass.PerfiList = PerfiList;
                    usrPerfil winPerfil = new usrPerfil();
                    winPerfil.Show();
                });
            }
        }
    }
}