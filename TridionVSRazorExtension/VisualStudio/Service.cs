using EnvDTE;

namespace SDL.TridionVSRazorExtension.VisualStudio
{
    public static class Service
    {
        public static Project GetConfiguredProject(Solution solution)
        {
            foreach (Project project in solution.Projects)
            {
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    if (projectItem.Name.EndsWith("TridionRazorMapping.xml"))
                        return project;
                }
            }

            return null;
        }

        public static Project GetCurrentProject(DTE application)
        {
            foreach (SelectedItem selectedItem in application.SelectedItems)
            {
                return selectedItem.ProjectItem.ContainingProject;
            }

            return null;
        }

        public static Project GetProject(DTE application, Solution solution)
        {
            Project project = GetConfiguredProject(solution);
            if (project != null)
                return project;

            project = GetCurrentProject(application);
            if (project != null)
                return project;

            return solution.Projects.Item(1);
        }

    }
}
