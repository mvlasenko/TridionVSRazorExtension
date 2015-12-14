using System;
using System.Linq;
using System.Web.Mvc;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Common.Misc;
using TcmDebugger.Engines;
using TcmDebugger.Service;
using Tridion.ContentManager;

namespace SDL.RazorMediatorLayouts.Controllers
{
    public class DefaultController : Controller
    {
        //http://localhost/Views/ComponentLayouts/FileName.cshtml
        public ActionResult GetByComponentLayoutPath(string file)
        {
            string rootPath = Server.MapPath("~/");

            Configuration configuration = Service.GetConfiguration(rootPath, "TridionRazorMapping.xml");
            if(configuration == null)
                return View("~/Views/Shared/MappingError.cshtml");

            MappingInfo mapping = configuration.FirstOrDefault(x => x.Name == (configuration.DefaultConfiguration ?? "Default"));
            if (mapping == null)
                return View("~/Views/Shared/MappingError.cshtml");

            ProjectFolderInfo componentLayouts = mapping.ProjectFolders.FirstOrDefault(x => x.ProjectFolderRole == ProjectFolderRole.ComponentLayout);
            if (componentLayouts == null)
                return View("~/Views/Shared/MappingError.cshtml");

            ProjectFileInfo fileInfo = componentLayouts.ChildItems.FirstOrDefault(x => x.Path == "Views\\ComponentLayouts\\" + file + ".cshtml") as ProjectFileInfo;
            if (fileInfo == null)
                return View("~/Views/Shared/MappingError.cshtml");

            return GetView(fileInfo.TestItemTcmId, fileInfo.TestTemplateTcmId, "~/Views/ComponentLayouts/" + file + ".cshtml");
        }

        //http://localhost/Views/PageLayouts/FileName.cshtml
        public ActionResult GetByPageLayoutPath(string file)
        {
            string rootPath = Server.MapPath("~/");

            Configuration configuration = Service.GetConfiguration(rootPath, "TridionRazorMapping.xml");
            if (configuration == null)
                return View("~/Views/Shared/MappingError.cshtml");

            MappingInfo mapping = configuration.FirstOrDefault(x => x.Name == (configuration.DefaultConfiguration ?? "Default"));
            if (mapping == null)
                return View("~/Views/Shared/MappingError.cshtml");

            ProjectFolderInfo PageLayouts = mapping.ProjectFolders.FirstOrDefault(x => x.ProjectFolderRole == ProjectFolderRole.PageLayout);
            if (PageLayouts == null)
                return View("~/Views/Shared/MappingError.cshtml");

            ProjectFileInfo fileInfo = PageLayouts.ChildItems.FirstOrDefault(x => x.Path == "Views\\PageLayouts\\" + file + ".cshtml") as ProjectFileInfo;
            if (fileInfo == null)
                return View("~/Views/Shared/MappingError.cshtml");

            return GetView(fileInfo.TestItemTcmId, fileInfo.TestTemplateTcmId, "~/Views/PageLayouts/" + file + ".cshtml");
        }

        //http://localhost/FileName/4042-29578/5057-30730-32
        public ActionResult GetByUri(string file, string item, string template)
        {
            ItemType itemType = GetItemType(item);

            string path = "~/Views/" + (itemType == ItemType.Component ? "ComponentLayouts" : "PageLayouts") + "/" + file + ".cshtml";

            return GetView("tcm:" + item, "tcm:" + template, path);
        }

        private ActionResult GetView(string itemUri, string templateUri, string path)
        {
            using (new PreviewServer())
            {
                DebugEngine engine = new DebugEngine();
                string output = engine.Execute(itemUri, templateUri);
                //now Engine and Template instances are created

                return View(path);
            }
        }

        public static ItemType GetItemType(string tcmItem)
        {
            string[] arr = tcmItem.Replace("tcm:", String.Empty).Split('-');
            if (arr.Length == 2) return ItemType.Component;

            return (ItemType)Int32.Parse(arr[2]);
        }

    }
}