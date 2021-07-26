using BertEngine;
using Gestion.DAL;
using Gestion.DAL.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Indexai
{
    internal class GlobalClass
    {
        public static string email { get; set; }
        public static string clave { get; set; }
        public static int id_usuario { get; set; }
        public static string nom_usuario { get; set; }
        public static string nombres { get; set; }
        public static string apellidos { get; set; }
        public static int id_proyecto { get; set; }
        public static string ruta_proyecto { get; set; }
        public static string ruta_salida { get; set; }
        public static string nom_proyecto { get; set; }
        public static int superadmin { get; set; }
        public static int loc_admin { get; set; }
        public static int loc_index { get; set; }
        public static int loc_calidad { get; set; }
        public static int loc_consulta { get; set; }
        public static string version { get; set; }
        public static int selPagInicial { get; set; }
        public static int selPagFinal { get; set; }
        public static CarpetaModel Carpeta { get; internal set; }
        public static List<gdperfil> PerfiList { get; set; }
        public static ICollection<t_modulo> Modulos { get; internal set; }
        public static string estructura_export { get; set; }
        public static string nombre_export { get; internal set; }
        public static string SortColumns { get; internal set; }
        public static MenuPrincipalInd ViewController { get; internal set; }
        public static int UserSelectedIndex { get; internal set; } = -1;
        public static bool FromIndexado { get; internal set; } = false;

        public static int GetNumber(string txt,int defecto = 0)
        {
            if (string.IsNullOrEmpty(txt)) return defecto;
            int res = defecto;
            var resultString = Regex.Match(txt, @"\d+").Value;
            if(!string.IsNullOrEmpty(resultString)) int.TryParse(resultString, out res);
            return res;
        }
    }
}