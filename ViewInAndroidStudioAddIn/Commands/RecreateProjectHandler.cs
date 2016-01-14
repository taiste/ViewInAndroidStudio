using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using System.Linq;
using MonoDevelop.Projects;
using Taiste.ViewInAndroidStudio.Util;

namespace Taiste.ViewInAndroidStudio.Commands
{
    public class RecreateProjectHandler: CommandHandler
    {
        protected override void Run ()
        {
            base.Run ();
            var project = IdeApp.ProjectOperations.CurrentSelectedItem as Project;
            if (project != null && project.IsAndroidProject()) {
                project.CreateAndroidStudioProject ();
            }
        }

        protected override void Update (CommandInfo info)
        {
            base.Update (info);
            var project = IdeApp.ProjectOperations.CurrentSelectedItem as Project;
            info.Visible = project != null && project.IsAndroidProject(); 
        }
    }
}

