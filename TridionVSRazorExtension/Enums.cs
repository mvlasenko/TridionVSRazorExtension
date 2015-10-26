namespace SDL.TridionVSRazorExtension
{
    public enum TridionRole
    {
        Other,
        PageLayoutContainer,
        ComponentLayoutContainer,
        PageTemplateContainer,
        ComponentTemplateContainer,
        MultimediaComponentContainer
    }

    public enum ProjectFolderRole
    {
        Other,
        PageLayout,
        ComponentLayout,
        Binary
    }

    public enum TridionSelectorMode
    {
        Folder,
        StructureGroup,
        FolderAndStructureGroup
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
