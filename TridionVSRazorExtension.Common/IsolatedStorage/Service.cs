using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace SDL.TridionVSRazorExtension.Common.IsolatedStorage
{
    public static class Service
    {
        public static string GetFromIsolatedStorage(string key)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url)))
            {
                if (!isf.FileExists(key + ".txt"))
                    return String.Empty;

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(key + ".txt", FileMode.Open, isf))
                {
                    using (StreamReader sr = new StreamReader(isfs, System.Text.Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        public static void SaveToIsolatedStorage(string key, string value)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url)))
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(key + ".txt", FileMode.Create, isf))
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(value);
                    isfs.Write(data, 0, data.Length);
                }
            }
        }

        public static string GetId(params object[] keys)
        {
            return String.Join("_", keys).Replace("tcm:", "").Replace("http:", "").Replace("https:", "").Replace("/", "").Replace("<", "").Replace(">", "").Replace("[", "").Replace("]", "").Replace("-", "").Replace(":", "_").Replace(" ", "");
        }

    }
}
