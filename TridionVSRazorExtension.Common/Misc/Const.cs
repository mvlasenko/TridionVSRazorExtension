using System.Collections.Generic;

namespace SDL.TridionVSRazorExtension.Common.Misc
{
    public static class Const
    {
        public static readonly Dictionary<string, string> Extensions = new Dictionary<string, string>
            {
                {".css", "text/css"},
                {".js", "text/javascript"},
                {".jpg", "image/jpg"},
                {".png", "image/png"},
                {".gif", "image/gif"},
                {".mp4", "video/mp4"},
                {".webm", "video/webm"},
                {".ogv", "video/ogg"},
                {".swf", "application/x-shockwave-flash"},
                {".eot", "application/vnd.ms-fontobject"},
                {".svg", "image/svg+xml"},
                {".ttf", "application/x-font-truetype"},
                {".woff", "application/x-font-woff"},
                {".pdf", "application/pdf"},
                {".doc", "application/msword"},
                {".xls", "application/vnd.ms-excel"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".kml", "application/vnd.google-earth.kml+xml"},
                {".kmz", "application/vnd.google-earth.kmz"},
                {".zip", "application/x-zip-compressed"},
                {".ico", "image/x-icon"}
            };

    }
}
