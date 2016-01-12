using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using System.Linq;

namespace ViewInAndroidStudio
{
    public class RecreateProjectHandler: CommandHandler
    {
        protected override void Run ()
        {
            base.Run ();
            var projects = IdeApp.Workspace.GetAllProjects ();
            foreach (var project in projects) {
                if (project.GetProjectTypes ().Contains ("MonoDroid")) {
                    ProjectHandler.CreateProject (project);
                }
            }
        }

        protected override void Update (CommandInfo info)
        {
            base.Update (info);
        }
    }
}

