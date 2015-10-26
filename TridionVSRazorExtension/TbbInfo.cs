using System.Xml.Linq;

namespace SDL.TridionVSRazorExtension
{
    public class TbbInfo
    {
        public string TcmId
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public XElement TemplateParameters
        {
            get;
            set;
        }
    }
}