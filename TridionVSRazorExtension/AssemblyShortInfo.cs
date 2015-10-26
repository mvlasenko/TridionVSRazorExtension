using System;

namespace SDL.TridionVSRazorExtension
{
    public class AssemblyShortInfo
    {
        public string Name { get; set; }
        public string FrameworkVersion { get; set; }
        public string Version { get; set; }
        public string Culture { get; set; }
        public string Token { get; set; }
        public string Path { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", this.Name, this.Version, String.IsNullOrEmpty(this.Culture) ? "neutral" : this.Culture, this.Token);
            }
        }
    }
}
