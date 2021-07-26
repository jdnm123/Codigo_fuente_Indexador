using NeoxzAI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NumbersHelper
{
    /// <summary>
    /// Conversión de números a sus velores enteros.
    /// </summary>
    public static class NumbersSymbolsHelper
    {
        /// <summary>
        /// Lista de números cargadas desde el JSON.
        /// </summary>
        private static List<Numbers> Numbers;

        /// <summary>
        /// Lista de números escritos en Hash para mayor velocidad.
        /// </summary>
        private static HashSet<string> NumbersHash;

        /// <summary>
        /// Diccionario con la lista de letras y su pronunciación.
        /// </summary>
        public static Dictionary<string, string> Chars { get; private set; }

        /// <summary>
        /// Intenta convertir a valor númerico los números escritos en el texto.
        /// </summary>
        /// <param name="transcription">Texto con posibles números.</param>
        /// <returns>Texto convertido a su equivalente numérico, mismo texto de entrada si la conversión falla.</returns>
        public static string ToNumber(this string transcription)
        {
            transcription = transcription.ToLower();
            try
            {
                if (Numbers == null)
                {
                    LoadNumbers();
                }
                var parts = transcription.Split(' ');
                if (parts.Length != 0 && parts.ToList().Any(x => NumbersHash.Contains(x)))
                {
                    foreach (var newNumber in Numbers)
                    {
                        //if (newNumber.Nombre != "mil")
                        //{
                            if (transcription.Contains(newNumber.Nombre))
                            {
                            string pattern = $@"\b{newNumber.Nombre}\b";
                            transcription = Regex.Replace(transcription, pattern, newNumber.Valor.ToString());

                            //transcription = transcription.Replace(oldValue: newNumber.Nombre, newValue: newNumber.Valor.ToString());
                            }
                        //}
                    }
                    char[] chars = transcription.ToArray();
                    bool allNumbers = true;
                    foreach (var charN in chars)
                    {
                        if (char.IsLetter(charN))
                        {
                            allNumbers = false;
                        }
                    }
                    if (allNumbers)
                    {
                        transcription = transcription.Replace(" ", "");
                    }
                }
                return Regex.Replace(transcription, "(?<=\\d) +(?=\\d)", "");
            }
            catch (Exception)
            {
                return transcription;
            }
        }

        /// <summary>
        /// Intenta convertir texto que contiene pronunciación de caracteres.
        /// </summary>
        /// <param name="transcription">Texto con posibles caracteres.</param>
        /// <returns>Texto convertido a su equivalente en caracteres, mismo texto de entrada si la conversión falla.</returns>
        public static string ParseToChar(this string transcription)
        {
            transcription = transcription.ToLower();
            try
            {
                if (Numbers == null)
                {
                    LoadNumbers();
                }
                if (Chars == null)
                {
                    LoadChars();
                }
                foreach (var replaceChar in Chars)
                {
                    if (replaceChar.Key.Contains(" "))
                    {
                        transcription = transcription.Replace(replaceChar.Key, replaceChar.Value);
                    }
                    else
                    {
                        var transcriptionParts = transcription.Split(' ');

                        for (int index = 0; index < transcriptionParts.Length; index++)
                        {
                            if (transcriptionParts[index] == replaceChar.Key)
                            {
                                transcriptionParts[index] = replaceChar.Value;
                            }
                        }
                        transcription = string.Join(" ", transcriptionParts);
                    }
                }

                var parts = transcription.Split(' ');



                if (parts.Length != 0 && parts.ToList().Any(x => NumbersHash.Contains(x)))
                {
                    foreach (var newumber in Numbers)
                    {
                        if (transcription.Contains(newumber.Nombre))
                        {
                            transcription = transcription.Replace(oldValue: newumber.Nombre, newValue: newumber.Valor.ToString());
                        }
                    }


                    transcription = transcription.Replace(" ", "");
                }


                return transcription;
            }
            catch (Exception)
            {
                return transcription;
            }
        }

        /// <summary>
        /// Carga la lista de letras y pronunciación a un diccionario.
        /// </summary>
        private static void LoadChars()
        {
            Chars = new Dictionary<string, string>();
            if (File.Exists("lista-letras.txt"))
            {
                var chars = File.ReadAllLines("lista-letras.txt", Encoding.UTF8);
                foreach (var lChar in chars)
                {
                    var parts = lChar.Split(',');
                    foreach (var synom in parts[1].Split('|'))
                    {
                        Chars.Add(synom, parts[0]);
                    }
                }
                Chars = Chars.OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value); //ordena para reemplazar primero las combinaciones de varias palabras
                //evita uve reemplace primero a doble uve
            }
            else
            {
                throw new Exception("Falta el archivo lista-letras.txt");
            }
        }

        /// <summary>
        /// Carga los números desde al archivo JSON.
        /// </summary>
        private static void LoadNumbers()
        {
            Numbers = JsonConvert.DeserializeObject<List<Numbers>>(File.ReadAllText("nums.json")).OrderBy(x => x.Valor).ToList();
            NumbersHash = Numbers.Select(x => x.Nombre).ToHashSet();
            Numbers.Reverse(); // Iniciar a reemplazar por los más grandes primero.
        }
        /// <summary>
        /// Convierte de símbolos escritos a su valor en símbolo.
        /// </summary>
        /// <param name="transcription"></param>
        /// <returns>Transcripción con los nuevos símbolos.</returns>
        public static string ReplaceSymbols(this string transcription)
        {
            LoadChars();
            
            transcription = transcription.Replace("numero", "No.");
            transcription = transcription.Replace("número", "No.");
            transcription = transcription.Replace("ve pequeña","v");
            transcription = transcription.Replace("be pequeña", "v");
            transcription = transcription.Replace("ve corta", "v");
            transcription = transcription.Replace("be corta", "v");
            //transcription = transcription.Replace("doble u ", "w");
            transcription = Regex.Replace(transcription,
           @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))" + "backslash" + @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))", @"\", RegexOptions.IgnoreCase);

            transcription = Regex.Replace(transcription,
           @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))" + "slash" + @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))", "/", RegexOptions.IgnoreCase);

            transcription = Regex.Replace(transcription,
           @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))" + "doble u" + @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))", "w", RegexOptions.IgnoreCase);

            transcription = Regex.Replace(transcription,
           @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))" + "doble be" + @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))", "w", RegexOptions.IgnoreCase);

            transcription = Regex.Replace(transcription,
            @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))" + "doble ve" + @"(?:(?<=^|\s)(?=\S|$)|(?<=^|\S)(?=\s|$))", "w", RegexOptions.IgnoreCase);

            var words = transcription.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word == "guión" || word == "guion")
                {
                    words[i] = "-";
                }
                else if (word == "punto")
                {
                    words[i] = ".";
                }
                else if (word == "coma")
                {
                    words[i] = ",";
                }

                foreach (var letra in Chars)
                {
                    if (word == letra.Key && word != "de")
                    {
                        words[i] = letra.Value;
                    }
                }

            }
            var resultString = string.Join(" ", words);

            string pattern = @"(?i)(?<=\b[a-z]) (?=[a-z]\b)";
            resultString = Regex.Replace(resultString, pattern, "");
            resultString = resultString.Replace("espacio", "");
            resultString = Regex.Replace(resultString,
          @"(?:(?<=^|\d)(?=\d|$)|(?<=^|\d)(?=\s|$)) / (?:(?<=^|\s)(?=\S|$)|(?<=^|\d)(?=\d|$))", "/", RegexOptions.IgnoreCase);
             
            return resultString.ToUpper();
        }

    }
}