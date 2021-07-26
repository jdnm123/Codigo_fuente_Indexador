using System;
using System.Collections.Generic;
using System.Linq;

namespace NumbersHelper
{
    public static class DateConverter
    {
        public static Dictionary<string, string> Months = new Dictionary<string, string>
        {
            { "enero","01" },
            { "febrero","02" },
            { "marzo","03" },
            { "abril","04" },
            { "mayo","05" },
            { "junio","06" },
            { "julio","07" },
            { "agosto","08" },
            { "septiembre","09" },
            { "octubre","10" },
            { "noviembre","11" },
            { "diciembre","12" }
        };

        public static Dictionary<string, int> Replaces = new Dictionary<string, int>();

        /// <summary>
        /// Convierte texto con fecha sin formato a formato de fecha numérico (DD/MM/YYYY).
        /// </summary>
        /// <param name="inputText">Fecha sin formato.</param>
        /// <returns>String con fecha en formato (DD/MM/YYYY).</returns>
        public static string ToDateFormat(this string inputText)
        {
            if (Replaces.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    Replaces.Add(NeoxzAI.NeoxzMathControllerLib.ParseNumber(i).Trim(),i);
                }
                Replaces = Replaces.OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value);
            }
            try
            {
                var dateParts = inputText.Split(' ').Where(x => x != "de" && x != "del").ToArray(); //elimina palabras no requeridas para la fecha
                string newDate = string.Empty;
                for (int i = 0; i < dateParts.Length; i++)
                {
                    string word = dateParts[i];
                    if (Months.ContainsKey(word.ToLower()))
                    {
                        dateParts[i] = $"/{Months[word.ToLower()]}/"; //convierte el mes a número
                    }
                    else if (word == "primero") //se  convierte las fechas donde se menciona primero convirtiendo a uno, uno será convertido a 1
                    {
                        dateParts[i] = "uno";
                    }
                }
                string firstPart = string.Empty;
                string secondPart = string.Empty;
                string month = string.Empty;
                for (int i = 0; i < dateParts.Length; i++)
                {
                    string item = dateParts[i];
                    if (item.Contains("/"))
                    {
                        month = item;
                        firstPart = string.Join(" ", dateParts.ToList().GetRange(0, i));
                        secondPart = string.Join(" ", dateParts.ToList().GetRange(i + 1,  dateParts.Length-(i+1)));
                        break;
                    }
                }

                newDate = $"{NeoxzAI.NeoxzMathControllerLib.ParseToNumber(firstPart)}{month}{NeoxzAI.NeoxzMathControllerLib.ParseToNumber(secondPart)}";
                
               
                return newDate;
            }
            catch (System.Exception ex)
            {
                foreach (var replace in Replaces)
                {
                    inputText = inputText.Replace(replace.Key, " "+replace.Value.ToString()+" ");
                }
                string[] vs = inputText.Split(' ').Where(x=>!string.IsNullOrEmpty(x)).ToArray();
                string fecha = string.Join(" ", vs.Select(x => ToNumber(x)));
                return fecha;
            }
        }

        private static long ToNumber(string x)
        {
            try
            {
                return NeoxzAI.NeoxzMathControllerLib.ParseToNumber(x);
            }
            catch (Exception)
            {
                return Convert.ToInt64( x.Trim());
            }
        }

        public static string ToChar(this string inputText)
        {
            try
            {
                Dictionary<string, List<string>> chars = new Dictionary<string, List<string>>();

                chars.Add(key: "v", value: new List<string> { "uve", "ve pequeña" });


                var splitChar = "uve|ve pequeña".Split('|').ToList();
                return "";
            }
            catch (System.Exception ex)
            {
                return string.Empty;
            }
        }
    }
}