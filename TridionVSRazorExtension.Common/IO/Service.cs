using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SDL.TridionVSRazorExtension.Common.IO
{
    public static class Service
    {
        public static string[] GetFiles(string dir, string[] allowedExtensions)
        {
            if (String.IsNullOrEmpty(dir) || allowedExtensions == null)
                return new string[] { };

            List<string> res = new List<string>();
            foreach (string extension in allowedExtensions)
            {
                res.AddRange(Directory.GetFiles(dir, "*" + extension));
            }

            res.Sort();

            return res.ToArray();
        }

        public static List<AssemblyShortInfo> GetAssemblies()
        {
            List<AssemblyShortInfo> res = new List<AssemblyShortInfo>();

            List<string> assemblyFolders = new List<string>
            {
                @"C:\Windows\assembly",
                @"C:\Windows\Microsoft.NET\assembly"
            };

            List<string> gacFolders = new List<string>
            {
                "GAC", "GAC_32", "GAC_64", "GAC_MSIL",
                "NativeImages_v2.0.50727_32",
                "NativeImages_v2.0.50727_64",
                "NativeImages_v4.0.30319_32",
                "NativeImages_v4.0.30319_64"
            };

            foreach (string folder1 in assemblyFolders)
            {
                foreach (string folder2 in gacFolders)
                {
                    string path = Path.Combine(folder1, folder2);
                    if (Directory.Exists(path))
                    {
                        foreach (string assemblyNamePath in Directory.GetDirectories(path))
                        {
                            string name = Path.GetFileName(assemblyNamePath);
                            if (String.IsNullOrEmpty(name))
                                continue;

                            foreach (string assemblyVersionPath in Directory.GetDirectories(assemblyNamePath))
                            {
                                //example v4.0_1.0.0.0__31bf3856ad364e35
                                string assemblyVersionName = Path.GetFileName(assemblyVersionPath);
                                if (String.IsNullOrEmpty(assemblyVersionName))
                                    continue;

                                string fwVersion = null;
                                string version = null;
                                string culture = null;
                                string token;

                                if (assemblyVersionName.Contains("_"))
                                {
                                    string[] arr = assemblyVersionName.Split('_');

                                    if (assemblyVersionName.StartsWith("v"))
                                    {
                                        fwVersion = arr[0].Replace("v", "");
                                        version = arr[1];
                                        culture = arr[2];
                                        token = arr[3];
                                    }
                                    else
                                    {
                                        version = arr[0];
                                        culture = arr[1];
                                        token = arr[2];
                                    }
                                }
                                else
                                {
                                    token = assemblyVersionName;
                                }

                                foreach (string assemblyPath in Common.IO.Service.GetFiles(assemblyVersionPath, new[] { ".dll", ".exe" }))
                                {
                                    res.Add(new AssemblyShortInfo
                                    {
                                        Name = name,
                                        FrameworkVersion = fwVersion,
                                        Version = version,
                                        Culture = culture,
                                        Token = token,
                                        Path = assemblyPath
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        private static List<AssemblyShortInfo> _allAssemblies;
        public static string GetAssembly(string name)
        {
            if (_allAssemblies == null)
                _allAssemblies = GetAssemblies();

            AssemblyShortInfo assembly = _allAssemblies.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (assembly == null)
                return null;

            return assembly.FullName;
        }

    }
}
