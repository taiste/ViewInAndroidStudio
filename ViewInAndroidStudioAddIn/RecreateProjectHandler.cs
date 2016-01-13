using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using System.Linq;
using MonoDevelop.Projects;

namespace Taiste.ViewInAndroidStudio
{
    public class RecreateProjectHandler: CommandHandler
    {
        protected override void Run ()
        {
            base.Run ();
            var project = IdeApp.ProjectOperations.CurrentSelectedItem as Project;
            if (project != null && project.GetProjectTypes ().Contains ("MonoDroid")) {
                ProjectHelpers.CreateAndroidStudioProject (project);
            }
        }

        protected override void Update (CommandInfo info)
        {
            base.Update (info);
            var file = IdeApp.ProjectOperations.CurrentSelectedItem as Project;
            info.Visible = file != null && file.GetProjectTypes ().Contains ("MonoDroid"); 
        }
    }
}

