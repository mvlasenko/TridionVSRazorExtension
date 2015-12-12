using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension.Common.Configuration
{
    public static class Service
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Configuration));

        public static void SaveConfiguration(string rootPath, string name, Configuration configuration)
        {
            if (configuration == null)
                return;

            Configuration clearedConfiguration = configuration.ClearConfiguration();
            if (clearedConfiguration == null)
                return;

            string path = Path.Combine(rootPath, name);

            if (File.Exists(path))
            {
                string xml;
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Auto, OmitXmlDeclaration = false };
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var writer = XmlWriter.Create(ms, settings))
                    {
                        Serializer.Serialize(writer, clearedConfiguration);
                    }
                    xml = Encoding.UTF8.GetString(ms.ToArray());
                }

                //replace code
                File.WriteAllText(path, xml);
            }
            else
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Auto, OmitXmlDeclaration = false };
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = XmlWriter.Create(fs, settings))
                    {
                        Serializer.Serialize(writer, clearedConfiguration);
                    }
                }
            }
        }

        public static Configuration ClearConfiguration(this Configuration configuration)
        {
            Configuration clearedConfiguration = new Configuration();
            clearedConfiguration.AddRange(configuration.Select(mapping => mapping.ClearMapping()));
            return clearedConfiguration;
        }

        public static MappingInfo ClearMapping(this MappingInfo mapping)
        {
            MappingInfo clearedMapping = new MappingInfo();
            clearedMapping.Name = mapping.Name;
            clearedMapping.Host = mapping.Host;
            clearedMapping.Username = mapping.Username;
            clearedMapping.Password = mapping.Password;
            clearedMapping.TimeZoneId = mapping.TimeZoneId;
            clearedMapping.TridionFolders = mapping.TridionFolders;

            if (mapping.ProjectFolders != null && mapping.ProjectFolders.Count > 0)
            {
                clearedMapping.ProjectFolders = new List<ProjectFolderInfo>();
                foreach (ProjectFolderInfo projectFolder in mapping.ProjectFolders)
                {
                    ProjectFolderInfo clearedProjectFolder = projectFolder.ClearProjectFolder(false);
                    if (clearedProjectFolder != null)
                        clearedMapping.ProjectFolders.Add(clearedProjectFolder);
                }
            }

            return clearedMapping;
        }

        public static ProjectFolderInfo ClearProjectFolder(this ProjectFolderInfo projectFolder, bool checkChecked)
        {
            if (projectFolder == null)
                return null;

            if (checkChecked && projectFolder.Checked == false)
                return null;

            ProjectFolderInfo clearedProjectFolder = new ProjectFolderInfo();
            clearedProjectFolder.Path = projectFolder.Path;
            clearedProjectFolder.Checked = projectFolder.Checked;
            clearedProjectFolder.SyncTemplate = projectFolder.SyncTemplate;
            clearedProjectFolder.TemplateFormat = projectFolder.TemplateFormat;
            clearedProjectFolder.ProjectFolderRole = projectFolder.ProjectFolderRole;
            clearedProjectFolder.TcmId = projectFolder.TcmId;

            if (projectFolder.ChildItems != null && projectFolder.ChildItems.Count > 0)
            {
                clearedProjectFolder.ChildItems = new List<ProjectItemInfo>();
                foreach (ProjectItemInfo childItem in projectFolder.ChildItems)
                {
                    if (childItem.IsFolder)
                    {
                        ProjectFolderInfo clearedChildProjectFolder = ((ProjectFolderInfo)childItem).ClearProjectFolder(true);
                        if (clearedChildProjectFolder != null)
                            clearedProjectFolder.ChildItems.Add(clearedChildProjectFolder);
                    }
                    if (childItem.IsFile)
                    {
                        if (childItem.Checked == true)
                        {
                            clearedProjectFolder.ChildItems.Add(childItem);
                        }
                    }
                }
                if (clearedProjectFolder.ChildItems.Count == 0)
                    clearedProjectFolder.ChildItems = null;
            }

            return clearedProjectFolder;
        }

        public static Configuration GetConfiguration(string rootPath, string name)
        {
            string path = Path.Combine(rootPath, name);
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Configuration configuration = (Configuration)Serializer.Deserialize(fs);

                //set root path property to chind items
                foreach (MappingInfo mapping in configuration)
                {
                    foreach (ProjectFolderInfo folder in mapping.ProjectFolders)
                    {
                        SetRootPath(folder, rootPath);
                    }
                }

                //merge with file system
                foreach (MappingInfo mapping in configuration)
                {
                    foreach (ProjectFolderInfo projectFolder in mapping.ProjectFolders)
                    {
                        AddChildItems(projectFolder, rootPath);
                    }
                }

                return configuration;
            }
        }

        public static void SetRootPath(ProjectFolderInfo folder, string rootPath)
        {
            folder.RootPath = rootPath;
            if (folder.ChildItems != null)
            {
                foreach (ProjectItemInfo childItem in folder.ChildItems)
                {
                    childItem.Parent = folder;
                    if (childItem.IsFile)
                    {
                        childItem.RootPath = rootPath;
                    }
                    if (childItem.IsFolder)
                    {
                        SetRootPath((ProjectFolderInfo)childItem, rootPath);
                    }
                }
            }
        }

        public static Configuration GetDefault(string rootPath)
        {
            Configuration res = new Configuration();
            res.DefaultConfiguration = "Default";

            MappingInfo mapping = GetDefaultMapping(rootPath, "Default");
            res.Add(mapping);

            return res;
        }

        public static MappingInfo GetDefaultMapping(string rootPath, string name)
        {
            MappingInfo mapping = new MappingInfo();
            mapping.Name = name;
            mapping.Host = "localhost";
            mapping.Username = "";
            mapping.Password = "";

            mapping.TridionFolders = new List<TridionFolderInfo>();
            mapping.TridionFolders.Add(new TridionFolderInfo { TridionRole = TridionRole.PageLayoutContainer, ScanForItems = true });
            mapping.TridionFolders.Add(new TridionFolderInfo { TridionRole = TridionRole.ComponentLayoutContainer, ScanForItems = true });
            mapping.TridionFolders.Add(new TridionFolderInfo { TridionRole = TridionRole.PageTemplateContainer });
            mapping.TridionFolders.Add(new TridionFolderInfo { TridionRole = TridionRole.ComponentTemplateContainer });

            mapping.ProjectFolders = new List<ProjectFolderInfo>();
            mapping.ProjectFolders.Add(new ProjectFolderInfo
            {
                ProjectFolderRole = Misc.ProjectFolderRole.PageLayout,
                Checked = true,
                RootPath = rootPath,
                Path = "Views\\PageLayouts",
                TemplateFormat =
                    "<CompoundTemplate xmlns=\"http://www.tridion.com/ContentManager/5.3/CompoundTemplate\"><TemplateInvocation><Template xlink:href=\"{0}\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xlink:title=\"{1}\" /><TemplateParameters></TemplateParameters></TemplateInvocation></CompoundTemplate>",
                SyncTemplate = true
            });
            mapping.ProjectFolders.Add(new ProjectFolderInfo
            {
                ProjectFolderRole = Misc.ProjectFolderRole.ComponentLayout,
                Checked = true,
                RootPath = rootPath,
                Path = "Views\\ComponentLayouts",
                TemplateFormat =
                    "<CompoundTemplate xmlns=\"http://www.tridion.com/ContentManager/5.3/CompoundTemplate\"><TemplateInvocation><Template xlink:href=\"{0}\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xlink:title=\"{1}\" /><TemplateParameters></TemplateParameters></TemplateInvocation></CompoundTemplate>",
                SyncTemplate = true
            });
            return mapping;
        }

        public static void DeleteFileFromMapping(ProjectFolderInfo folder, string filePath)
        {
            if (folder.ChildItems == null)
                return;

            folder.ChildItems.RemoveAll(x => x.IsFile && x.FullPath == filePath);

            foreach (ProjectFolderInfo childFolder in folder.ChildItems.Where(x => x.IsFolder))
            {
                DeleteFileFromMapping(childFolder, filePath);
            }
        }

        public static IEnumerable<ProjectFolderInfo> GetFileTree(ProjectFolderInfo projectFolder, string rootPath)
        {
            AddChildItems(projectFolder, rootPath);

            ProjectFolderInfo rootFolder = GetRootTree(projectFolder, rootPath);

            return new List<ProjectFolderInfo> { rootFolder };
        }

        public static void AddChildItems(ProjectFolderInfo projectFolder, string rootPath)
        {
            if (projectFolder == null)
                return;

            if (String.IsNullOrEmpty(projectFolder.FullPath))
                return;

            if (!Directory.Exists(projectFolder.FullPath))
                Directory.CreateDirectory(projectFolder.FullPath);

            ProjectFolderInfo topFolder = projectFolder.GetTopFolder();
            ProjectFolderRole role = topFolder.ProjectFolderRole;
            string[] extensions = role == Misc.ProjectFolderRole.Binary ? Const.Extensions.Keys.ToArray() : new[] { "*.cshtml" };
            string[] directories = Directory.GetDirectories(projectFolder.FullPath);
            string[] files = IO.Service.GetFiles(projectFolder.FullPath, extensions);

            if (directories.Length == 0 && files.Length == 0)
            {
                projectFolder.ChildItems = null;
                return;
            }

            List<ProjectItemInfo> newChildItems = new List<ProjectItemInfo>();

            foreach (string dir in directories)
            {
                ProjectFolderInfo childFolder = null;

                if (projectFolder.ChildItems != null)
                {
                    childFolder = projectFolder.ChildItems.FirstOrDefault(x => x.IsFolder && x.FullPath == dir) as ProjectFolderInfo;
                }

                if (childFolder == null)
                {
                    childFolder = new ProjectFolderInfo { RootPath = rootPath, Path = dir.Replace(rootPath, "").Trim('\\'), Checked = false };
                }

                childFolder.Parent = projectFolder;

                AddChildItems(childFolder, rootPath);

                newChildItems.Add(childFolder);
            }

            foreach (string file in files)
            {
                ProjectFileInfo childFile = null;

                if (projectFolder.ChildItems != null)
                {
                    childFile = projectFolder.ChildItems.FirstOrDefault(x => x.IsFile && x.FullPath == file) as ProjectFileInfo;
                }

                if (childFile == null)
                {
                    childFile = new ProjectFileInfo { RootPath = rootPath, Path = file.Replace(rootPath, "").Trim('\\'), Checked = false };
                }

                childFile.Parent = projectFolder;

                newChildItems.Add(childFile);
            }

            projectFolder.ChildItems = newChildItems.Count > 0 ? newChildItems : null;
        }

        public static ProjectFolderInfo GetRootTree(ProjectFolderInfo projectFolder, string fullPath)
        {
            if (projectFolder == null)
                return null;

            if (!Directory.Exists(fullPath))
                return null;

            if (!ExistFile(projectFolder, fullPath) && projectFolder.FullPath != fullPath)
                return null;

            if (projectFolder.FullPath == fullPath)
            {
                projectFolder.IsSelected = true;
                projectFolder.Expand();
                return projectFolder;
            }

            string rootPath = projectFolder.RootPath;

            ProjectFolderInfo parentFolder = new ProjectFolderInfo { RootPath = rootPath, Path = fullPath.Replace(rootPath, "").Trim('\\'), Checked = false };
            parentFolder.ChildItems = new List<ProjectItemInfo>();

            foreach (string childFolderPath in Directory.GetDirectories(fullPath))
            {
                ProjectFolderInfo childFolder = GetRootTree(projectFolder, childFolderPath);
                if (childFolder != null)
                {
                    childFolder.Parent = parentFolder;
                    parentFolder.ChildItems.Add(childFolder);
                }
            }

            foreach (string childFilePath in IO.Service.GetFiles(fullPath, projectFolder.ProjectFolderRole == Misc.ProjectFolderRole.Binary ? Const.Extensions.Keys.ToArray() : new[] { "*.cshtml" }))
            {
                ProjectFileInfo childFile = new ProjectFileInfo { RootPath = rootPath, Path = childFilePath.Replace(rootPath, "").Trim('\\'), Checked = false };
                childFile.Parent = parentFolder;
                parentFolder.ChildItems.Add(childFile);
            }

            if (parentFolder.ChildItems.Count == 0)
                parentFolder.ChildItems = null;

            return parentFolder;
        }

        public static ProjectFolderInfo GetSelectedFolderFromTree(IEnumerable<ProjectFolderInfo> tree)
        {
            foreach (ProjectFolderInfo projectFolder in tree)
            {
                if (projectFolder == null)
                    continue;

                if (projectFolder.Checked != false)
                {
                    return projectFolder;
                }

                if (projectFolder.ChildItems == null)
                    continue;

                var res = GetSelectedFolderFromTree(projectFolder.ChildItems.Where(x => x.IsFolder).Cast<ProjectFolderInfo>().ToList());
                if (res != null)
                    return res;
            }
            return null;
        }


        public static bool ExistFile(ProjectFolderInfo projectFolder, string dir, string extension)
        {
            return Directory.GetFiles(dir, "*" + extension).Any() || Directory.GetDirectories(dir).Any(x => ExistFile(projectFolder, x, extension));
        }

        public static bool ExistFile(ProjectFolderInfo projectFolder, string dir)
        {
            if (projectFolder.ProjectFolderRole == Misc.ProjectFolderRole.Binary)
            {
                return Const.Extensions.Keys.Select(extension => ExistFile(projectFolder, dir, extension)).Any(x => x);
            }

            return ExistFile(projectFolder, dir, "*.cshtml");
        }

        public static void Expand(this ProjectFolderInfo folder)
        {
            folder.IsExpanded = true;
            if (folder.Parent != null)
                folder.Parent.Expand();
        }
        
        public static List<ProjectFileInfo> GetFiles(this MappingInfo mapping, string filePath)
        {
            List<ProjectFileInfo> res = new List<ProjectFileInfo>();

            if (mapping == null)
                return res;

            foreach (ProjectFolderInfo projectFolder in mapping.ProjectFolders)
            {
                if (projectFolder.ChildItems == null)
                    continue;

                foreach (ProjectFileInfo file in projectFolder.ChildItems.Where(x => x.IsFile))
                {
                    if (file.FullPath == filePath && file.IsChecked())
                    {
                        file.Parent = projectFolder;
                        res.Add(file);
                    }
                }

                foreach (ProjectFolderInfo folder in projectFolder.ChildItems.Where(x => x.IsFolder))
                {
                    res.AddRange(folder.GetFiles(filePath));
                }
            }

            return res;
        }

        public static List<ProjectFileInfo> GetFiles(this ProjectFolderInfo folder, string filePath)
        {
            List<ProjectFileInfo> res = new List<ProjectFileInfo>();

            if (folder.ChildItems == null)
                return res;

            foreach (ProjectFileInfo file in folder.ChildItems.Where(x => x.IsFile))
            {
                if (file.FullPath == filePath && file.IsChecked())
                {
                    file.Parent = folder;
                    res.Add(file);
                }
            }

            foreach (ProjectFolderInfo childFolder in folder.ChildItems.Where(x => x.IsFolder))
            {
                res.AddRange(childFolder.GetFiles(filePath));
            }

            return res;
        }

        public static List<ProjectFolderInfo> GetFolders(this MappingInfo mapping, string filePath)
        {
            List<ProjectFolderInfo> res = new List<ProjectFolderInfo>();

            if (mapping == null)
                return res;

            foreach (ProjectFolderInfo projectFolder in mapping.ProjectFolders)
            {
                if (projectFolder.ChildItems == null)
                    continue;

                foreach (ProjectFileInfo file in projectFolder.ChildItems.Where(x => x.IsFile))
                {
                    if (file.FullPath == filePath && file.IsChecked())
                    {
                        res.Add(projectFolder);
                    }
                }

                foreach (ProjectFolderInfo folder in projectFolder.ChildItems.Where(x => x.IsFolder))
                {
                    res.AddRange(folder.GetFolders(filePath));
                }
            }

            return res;
        }

        public static List<ProjectFolderInfo> GetFolders(this ProjectFolderInfo folder, string filePath)
        {
            List<ProjectFolderInfo> res = new List<ProjectFolderInfo>();

            if (folder.ChildItems == null)
                return res;

            foreach (ProjectFileInfo file in folder.ChildItems.Where(x => x.IsFile))
            {
                if (file.FullPath == filePath && file.IsChecked())
                {
                    res.Add(folder);
                }
            }

            foreach (ProjectFolderInfo childFolder in folder.ChildItems.Where(x => x.IsFolder))
            {
                res.AddRange(childFolder.GetFolders(filePath));
            }

            return res;
        }

        public static List<ProjectFolderInfo> GetFoldersByPath(this MappingInfo mapping, string path)
        {
            List<ProjectFolderInfo> res = new List<ProjectFolderInfo>();

            foreach (ProjectFolderInfo projectFolder in mapping.ProjectFolders)
            {
                if (projectFolder.FullPath.Trim('\\') == path.Trim('\\') && projectFolder.Checked != false)
                {
                    res.Add(projectFolder);
                }

                res.AddRange(projectFolder.GetFoldersByPath(path));
            }
            return res;
        }

        public static List<ProjectFolderInfo> GetFoldersByPath(this ProjectFolderInfo folder, string path)
        {
            List<ProjectFolderInfo> res = new List<ProjectFolderInfo>();

            if (folder.ChildItems == null)
                return res;

            foreach (ProjectFolderInfo childFolder in folder.ChildItems.Where(x => x.IsFolder))
            {
                if (childFolder.FullPath.Trim('\\') == path.Trim('\\') && childFolder.Checked != false)
                {
                    res.Add(folder);
                }

                res.AddRange(childFolder.GetFoldersByPath(path));
            }

            return res;
        }

        public static bool IsChecked(this ProjectFileInfo file)
        {
            if (file == null)
                return false;

            return file.Checked == true;
        }

        public static bool IsChecked(this ProjectFolderInfo folder)
        {
            if (folder == null || folder.ChildItems == null)
                return false;

            return folder.ChildItems.Any(x => x.IsChecked());
        }

        public static bool IsChecked(this ProjectItemInfo item)
        {
            if (item.IsFolder)
                return ((ProjectFolderInfo)item).IsChecked();

            if (item.IsFile)
                return ((ProjectFileInfo)item).IsChecked();

            return false;
        }

        public static bool IsSyncTemplate(this ProjectFileInfo file)
        {
            if (file == null)
                return false;

            return file.SyncTemplate == true;
        }

        public static bool IsSyncTemplate(this ProjectFolderInfo folder)
        {
            if (folder == null || folder.ChildItems == null)
                return false;

            return folder.ChildItems.Any(x => x.IsSyncTemplate());
        }

        public static bool IsSyncTemplate(this ProjectItemInfo item)
        {
            if (item.IsFolder)
                return ((ProjectFolderInfo)item).IsSyncTemplate();

            if (item.IsFile)
                return ((ProjectFileInfo)item).IsSyncTemplate();

            return false;
        }

        public static string TemplateFormat(this ProjectFileInfo file)
        {
            ProjectFolderInfo folder = GetTopFolder(file);
            return folder != null ? folder.TemplateFormat : String.Empty;
        }

        public static ProjectFolderRole ProjectFolderRole(this ProjectFileInfo file)
        {
            ProjectFolderInfo folder = GetTopFolder(file);
            return folder != null ? folder.ProjectFolderRole : Misc.ProjectFolderRole.ComponentLayout;
        }

        public static ProjectFolderInfo GetTopFolder(this ProjectItemInfo item)
        {
            if (item == null)
                return null;

            ProjectFolderInfo parent = item.Parent;
            if ((parent == null || String.IsNullOrEmpty(parent.Path) || parent.ProjectFolderRole == Misc.ProjectFolderRole.Other) && item.IsFolder)
                return item as ProjectFolderInfo;

            return GetTopFolder(parent);
        }

        public static bool ExistsTcmId(this ProjectFolderInfo folder, string tcmId)
        {
            if (folder == null || folder.ChildItems == null)
                return false;

            foreach (ProjectItemInfo item in folder.ChildItems)
            {
                if (item.IsFile)
                {
                    if (tcmId == item.TcmId)
                        return true;
                }
                if (item.IsFolder)
                {
                    if (ExistsTcmId((ProjectFolderInfo)item, tcmId))
                        return true;
                }
            }
            return false;
        }
    }
}
