using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Linq;
using MonoDevelop.Ide.Tasks;
using System.Collections.Generic;
using MonoDevelop.Core;

namespace ViewInAndroidStudio
{
    public class ViewHandler : CommandHandler
    {

       protected override void Run ()
        {
            base.Run ();
            //TODO: implement
        }
            

        protected override void Update (CommandInfo info)
        {
            base.Update (info);
            info.Visible = ProjectFileUtils.IsResourceXmlFile(IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile);
        }

      
    }
}

