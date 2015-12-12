using System;

namespace SDL.TridionVSRazorExtension.Misc
{
    [Flags]
    public enum TridionSelectorMode
    {
        None = 0,
        Publication = 1,
        Folder = 2,
        StructureGroup = 4,
        Schema = 8,
        Component = 16,
        ComponentTemplate = 32,
        Page = 64,
        PageTemplate = 128,
        TargetGroup = 256,
        Category = 512,
        Keyword = 1024,
        TemplateBuildingBlock = 2048,
        Any = 8192,
    }

    public enum SchemaType
    {
        Any,
        Component,
        Metadata,
        Embedded,
        Multimedia,
        Parameters,
        Bundle,
        None
    }

    public enum SyncState
    {
        Tridion2VS,
        VS2Tridion,
        None
    }

    public enum BindingType
    {
        HttpBinding,
        TcpBinding
    }

    public enum LinkStatus
    {
        Found,
        NotFound,
        Mandatory,
        Error
    }
}
