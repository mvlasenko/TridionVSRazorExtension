using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SDL.TridionVSRazorExtension.Common.Misc
{
    public static class TextHelper
    {
        public static string RemoveSpaces(this string str)
        {
            string res = Regex.Replace(str, "\\s+\\r\\n", "\r\n");
            res = Regex.Replace(res, "\\t", "    ");
            return res.Trim();
        }

        public static string CutPath(this string path, string separator, int maxLength)
        {
            if (path == null || path.Length <= maxLength)
                return path;

            var list = path.Split(new[] { separator[0] });
            int itemMaxLength = maxLength / list.Length;

            return string.Join(separator, list.Select(item => item.Cut(itemMaxLength)).ToList());
        }

        public static string CutPath(this string path, string separator, int maxLength, bool fullLastItem)
        {
            if (path == null || path.Length <= maxLength)
                return path;

            if (!fullLastItem)
                return path.CutPath(separator, maxLength);

            string lastItem = path.Substring(path.LastIndexOf(separator, StringComparison.Ordinal));

            if (lastItem.Length > maxLength)
                return path.CutPath(separator, maxLength);

            return path.Substring(0, path.LastIndexOf(separator, StringComparison.Ordinal)).CutPath(separator, maxLength - lastItem.Length) + lastItem;
        }

        public static string Cut(this string str, int maxLength)
        {
            if (maxLength < 5)
                maxLength = 5;

            if (str.Length > maxLength)
            {
                return str.Substring(0, maxLength - 2) + "..";

            }
            return str;
        }

        public static string PrettyXml(this string xml)
        {
            try
            {
                return XElement.Parse(xml).ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static string PlainXml(this string xml)
        {
            try
            {
                return Regex.Replace(xml, "\\s+", " ").Replace("> <", "><");
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static string GetDomainName(this string url)
        {
            if (!url.Contains(Uri.SchemeDelimiter))
            {
                url = string.Concat(Uri.UriSchemeHttp, Uri.SchemeDelimiter, url);
            }
            Uri uri = new Uri(url);
            return uri.Host;
        }

        public static string GetMimeType(this string url)
        {
            string extension = Path.GetExtension(url);
            if (String.IsNullOrEmpty(extension))
                return String.Empty;

            if (Const.Extensions.Any(x => x.Key == extension))
                return Const.Extensions[extension];

            return String.Empty;
        }

        public static bool IsAllowedMimeType(this string url)
        {
            return !String.IsNullOrEmpty(url.GetMimeType());
        }

    }
}
