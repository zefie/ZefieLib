using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Zefie
{
    public static class Directory
    {
        /// <summary>
        /// List of supported file extensions
        /// </summary>
        public static List<string> supportedExtensions = new List<string>();

        /// <summary>
        /// Checks whether or not the filename matches extensions defined in supportedExtensions
        /// </summary>
        /// <param name="file">Filename to check against supportedExtensions</param>
        /// <returns>True if matches, false otherwise. If no supportedExtensions are defined, this will always return true.</returns>
        public static bool isSupported(string file)
        {
            if (supportedExtensions.Count() > 0)
            {
                string r = "";
                for (int i = 0; i < supportedExtensions.Count(); i++)
                    r += supportedExtensions[i] + "|";

                r = r.TrimEnd('|');
                if (String.IsNullOrEmpty(file))
                    return false;
                Regex ser = new Regex(@"\.(" + r + ")", RegexOptions.IgnoreCase);
                return ser.IsMatch(System.IO.Path.GetExtension(file) ?? String.Empty);
            }
            return true;
        }

        /// <summary>
        /// An extension of System.IO.Directory.GetFiles, this offers a natural sorting algorithm, RegEx searching, and defing an array of supported extensions.
        /// </summary>
        /// <param name="path">Directory to scan for files</param>
        /// <param name="search_string">RegExp search string</param>
        /// <param name="sort">Use natural sorting</param>
        /// <param name="recursive">Recursively scan directories</param>
        /// <returns>Array of files that match the search pattern (if any), and Zefie.Directory.supportedExtensions</returns>
        public static IEnumerable<FileInfo> GetFiles(string path, string search_string = null, bool sort = true, bool recursive = false)
        {
            if (!System.IO.Directory.Exists(path))
                return null;

            SearchOption so = SearchOption.TopDirectoryOnly;
            if (recursive)
                so = SearchOption.AllDirectories;

            string[] fs = null;
            if (sort)
            {
                if (search_string != null)
                {
                    Regex r = new Regex("(?i)" + search_string.Replace("*", ".*") + "(?-i)");
                    fs = System.IO.Directory.GetFiles(path, "*", so).Where(isSupported).Where(a => r.Match(a).Success).OrderByAlphaNumeric().ToArray();
                }
                else
                    fs = System.IO.Directory.GetFiles(path, "*", so).Where(isSupported).OrderByAlphaNumeric().ToArray();
            }
            else
                if (search_string != null)
                {
                    Regex r = new Regex("(?i)" + search_string.Replace("*", ".*") + "(?-i)");
                    fs = System.IO.Directory.GetFiles(path, "*", so).Where(isSupported).Where(a => r.Match(a).Success).OrderByAlphaNumeric().ToArray();
                }
                else
                fs = System.IO.Directory.GetFiles(path, "*", so).Where(isSupported).ToArray();

            FileInfo[] files = new FileInfo[fs.Count()];
            for (int i = 0; i < fs.Count(); i++)
                files[i] = new FileInfo(fs[i]);

            return files;
        }
        internal static IEnumerable<string> OrderByAlphaNumeric(this IEnumerable<string> source)
        {
            return OrderByAlphaNumeric(source, t => t);
        }

        internal static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            int max = source
                .SelectMany(i => Regex.Matches(selector(i), @"\d+").Cast<Match>().Select(m => (int?)m.Value.Length))
                .Max() ?? 0;

            return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0')));
        }
    }
}
