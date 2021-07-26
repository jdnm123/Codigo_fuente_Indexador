namespace Indexai.Helpers
{
    internal static class Cleaner
    {
        /// <summary>
        /// Elimina el acento.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string RemoveAccent(this string word)
        {
            return word.ToLower().Replace('á', 'a')
                .Replace('é', 'e')
                .Replace('í', 'i')
                .Replace('ó', 'o')
                .Replace('ú', 'u');
        }
    }
}
